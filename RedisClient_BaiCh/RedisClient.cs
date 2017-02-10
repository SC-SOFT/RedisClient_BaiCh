using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using RedisClient_BaiCh.CmdReturnEntities;
using RedisClient_BaiCh.Config;

namespace RedisClient_BaiCh
{
    public class RedisClient : IDisposable
    {
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIp { get; set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Auth { get; set; }

        private readonly Stopwatch _commandStopWatch = new Stopwatch();
        //private const int BufferSize = 4;//1024 * 256;  //接收数据的缓冲区大小byte
        private const int SendTimeout = 10000;  //发送超时时间ms
        private const int ReceiveTimeout = 10000;  //接收超时时间ms
        private const int HeartBeatInterval = 2000;  //心跳检测间隔ms

        private bool _disposeStarted; //是否已经开始释放

        private TcpClient _tcpClient;

        private readonly Mutex _mutex = new Mutex();

        /// <summary>
        /// 使用服务器IP，端口和密码初始化新的Redis客户端
        /// </summary>
        /// <param name="serverIp">服务器IP</param>
        /// <param name="serverPort">服务器端口</param>
        /// <param name="auth">服务器密码</param>
        public RedisClient(string serverIp, int serverPort, string auth)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
            Auth = auth;
            InitTcpClient();
            KeepConnection();
        }

        /// <summary>
        /// 使用服务器IP，端口初始化新的Redis客户端
        /// </summary>
        /// <param name="serverIp">服务器IP</param>
        /// <param name="serverPort">服务器端口</param>
        public RedisClient(string serverIp, int serverPort)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
            InitTcpClient();
            KeepConnection();
        }

        /// <summary>
        /// 使用配置文件初始化新的Redis客户端
        /// </summary>
        public RedisClient()
        {
            var config = ConfigurationManager.GetSection("RedisClient_BaiCh") as RedisClient_BaiChSection;
            if (config == null)
            {
                throw new Exception("config not found");
            }

            int intPort;
            try
            {
                intPort = int.Parse(config.Connection.Port);
            }
            catch (Exception)
            {
                throw new Exception("invalid port");
            }

            ServerIp = config.Connection.Server;
            ServerPort = intPort;
            Auth = config.Connection.Auth;

            InitTcpClient();

            KeepConnection();

        }

        /// <summary>
        /// 初始化TCP客户端，并进行验证（Auth）
        /// </summary>
        private void InitTcpClient()
        {
            _tcpClient = new TcpClient
            {
                SendTimeout = SendTimeout,
                ReceiveTimeout = ReceiveTimeout
            };
            _tcpClient.Connect(ServerIp, ServerPort);
            if (!string.IsNullOrEmpty(Auth))
            {
                AuthAsync().Wait();
            }
        }

        /// <summary>
        /// 进行密码验证
        /// </summary>
        /// <returns></returns>
        public Task<RedisCmdReturnAuth> AuthAsync()
        {
            if (string.IsNullOrEmpty(Auth))
            {
                throw new Exception("没有设置密码");
            }

            var rtn = new Task<RedisCmdReturnAuth>(delegate
            {

                var sb = new StringBuilder();
                sb.Append("Auth ");
                sb.Append(Auth);
                sb.Append("\r\n");

                var res = Command(sb.ToString());

                var redisCmdReturnAuth = new RedisCmdReturnAuth(res.First());

                return redisCmdReturnAuth;
            });
            rtn.Start();
            return rtn;
        }

        /// <summary>
        /// 设置指定键的值
        /// </summary>
        /// <param name="key">指定键</param>
        /// <param name="value">指定值</param>
        /// <param name="ex">过期时间（秒）</param>
        /// <param name="px">过期时间（毫秒）</param>
        /// <param name="nx">指定的key不存在的时候才会设置key的值</param>
        /// <param name="xx">指定的key存在的时候才会设置key的值</param>
        /// <returns></returns>
        public Task<RedisCmdReturnSet> SetAsync(string key, string value, int ex = -1, int px = -1, bool nx = false, bool xx = false)
        {
            var rtn = new Task<RedisCmdReturnSet>(delegate
            {

                var sb = new StringBuilder();
                sb.Append("set ");
                sb.Append(key);
                sb.Append(" \"");
                sb.Append(value);
                sb.Append("\"");

                if (ex!=-1)
                {
                    sb.Append(" ex ");
                    sb.Append(ex);
                }
                if (px != -1)
                {
                    sb.Append(" px ");
                    sb.Append(px);
                }
                if (nx)
                {
                    sb.Append(" nx");
                }
                if (xx)
                {
                    sb.Append(" xx");
                }

                sb.Append("\r\n");

                var res = Command(sb.ToString());
                var redisCmdReturnSet = new RedisCmdReturnSet(res.First());
                return redisCmdReturnSet;
            });
            rtn.Start();
            return rtn;
        }

        /// <summary>
        /// 设置多个键和其对应值，根据索引确定对应关系
        /// </summary>
        /// <param name="keys">包含所有键的数组</param>
        /// <param name="values">包含值的数组</param>
        /// <returns></returns>
        public Task<RedisCmdReturnMSet> MSetAsync(string[] keys, string[] values)
        {
            if (!keys.Any() ||
                (values.Length != keys.Length))
            {
                throw new Exception("参数keys和values必须是等长的非空数组");
            }

            var rtn = new Task<RedisCmdReturnMSet>(delegate
            {

                var sb = new StringBuilder();
                sb.Append("mset ");
                for (int i = 0; i < keys.Length; i++)
                {
                    sb.Append(keys[i]);
                    sb.Append(" \"");
                    sb.Append(values[i]);
                    sb.Append("\" ");
                }
                sb.Append("\r\n");

                var res = Command(sb.ToString());
                var redisCmdReturnMSet = new RedisCmdReturnMSet(res.First());
                return redisCmdReturnMSet;
            });
            rtn.Start();
            return rtn;
        }

        /// <summary>
        /// 获取制定键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public Task<RedisCmdReturnGet> GetAsync(string key)
        {
            var rtn = new Task<RedisCmdReturnGet>(delegate
            {

                var sb = new StringBuilder();
                sb.Append("get ");
                sb.Append(key);
                sb.Append("\r\n");
                var res = Command(sb.ToString());
                var redisCmdReturnGet = new RedisCmdReturnGet(res.First());
                return redisCmdReturnGet;
            });
            rtn.Start();
            return rtn;
        }

        /// <summary>
        /// 设置指定键的值并获取设置前的值
        /// </summary>
        /// <param name="key">制定键</param>
        /// <param name="value">新值</param>
        /// <returns></returns>
        public Task<RedisCmdReturnGetSet> GetSetAsync(string key, string value)
        {
            var rtn = new Task<RedisCmdReturnGetSet>(delegate
            {

                var sb = new StringBuilder();
                sb.Append("getset ");
                sb.Append(key);
                sb.Append(" \"");
                sb.Append(value);
                sb.Append("\"\r\n");
                var res = Command(sb.ToString());
                var redisCmdReturnGetSet = new RedisCmdReturnGetSet(res.First());
                return redisCmdReturnGetSet;
            });
            rtn.Start();
            return rtn;
        }

        /// <summary>
        /// 通知Redis关闭连接
        /// </summary>
        /// <returns></returns>
        public Task<RedisCmdReturnQuit> QuitAsync()
        {
            var rtn = new Task<RedisCmdReturnQuit>(delegate
            {

                var res = Command("Quit\r\n");
                var redisCmdReturnQuit = new RedisCmdReturnQuit(res.First());
                return redisCmdReturnQuit;
            });
            rtn.Start();
            return rtn;
        }

        /// <summary>
        /// 查找键，使用“*”返回所有键<br/>
        /// h?llo 匹配 hello, hallo 和 hxllo<br/>
        /// h* llo 匹配 hllo 和 heeeello<br/>
        /// h[ae]llo 匹配 hello 和 hallo 但是不匹配 hillo<br/>
        /// h[^ e]llo 匹配 hallo, hbllo 但是不匹配 hello<br/>
        /// h[a - b]llo 匹配 hallo 和 hbllo
        /// </summary>
        /// <param name="pattern">筛选条件</param>
        /// <returns></returns>
        public Task<RedisCmdReturnKeys> KeysAsync(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern) || string.IsNullOrEmpty(pattern))
            {
                throw new Exception("必须指定patter参数");
            }
            var rtn = new Task<RedisCmdReturnKeys>(delegate
            {
                var sb=new StringBuilder();
                sb.Append("keys ");
                sb.Append(pattern);
                sb.Append("\r\n");
                var res = Command(sb.ToString());
                var redisCmdReturnKeys = new RedisCmdReturnKeys(res.First());
                return redisCmdReturnKeys;
            });
            rtn.Start();
            return rtn;
        }

        /// <summary>
        /// 追加指定的值到指定的键
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <param name="value">指定的值</param>
        /// <returns></returns>
        public Task<RedisCmdReturnAppend> AppendAsync(string key, string value)
        {
            var rtn = new Task<RedisCmdReturnAppend>(delegate
            {
                var sb = new StringBuilder();
                sb.Append("append ");
                sb.Append(key);
                sb.Append(" ");
                sb.Append(value);
                sb.Append("\r\n");
                var res = Command(sb.ToString());
                var redisCmdReturnAppend = new RedisCmdReturnAppend(res.First());
                return redisCmdReturnAppend;
            });
            rtn.Start();
            return rtn;
        }

        /// <summary>
        /// 执行Redis命令
        /// </summary>
        /// <param name="command">要执行的命令（可以通过\r\n分割来使用Redis管道）</param>
        /// <returns></returns>
        public List<CommandMethodReturn> Command(string command)
        {
            var rtn = new List<CommandMethodReturn>();

            _mutex.WaitOne();
            var commandLine = command.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            var commandPadRN = new StringBuilder();
            foreach (string line in commandLine)
            {
                commandPadRN.Append(line);
                commandPadRN.Append("\r\n");
            }

            _commandStopWatch.Restart();
            _tcpClient.Client.Send(Encoding.UTF8.GetBytes(commandPadRN.ToString()));

            for (int i = 0; i < commandLine.Length; i++)
            {
                var commandMethodReturn = new CommandMethodReturn();

                var headBuffer = new byte[1];
                _tcpClient.Client.Receive(headBuffer);
                var headSymbol = Encoding.UTF8.GetString(headBuffer);
                switch (headSymbol)
                {
                    case "+":  //字符串回复
                        commandMethodReturn.SimpleString = ReadString2NextRN();
                        break;
                    case "-":  //异常回复
                        commandMethodReturn.Error = ReadString2NextRN();
                        break;
                    case ":":  //整数回复
                        commandMethodReturn.Integer = int.Parse(ReadString2NextRN());
                        break;
                    case "$":  //块回复
                        var length = ReadString2NextRN();
                        if (length == "-1")
                        {
                            break;
                        }
                        //var content = Read2NextRN();
                        var content = ReadSpecialBytes(int.Parse(length) + 2);  //+2意在读取最后的\r\n
                        commandMethodReturn.BulkStrings = content;
                        break;
                    case "*":  //多块回复
                        var blockCount = int.Parse(ReadString2NextRN());
                        if (blockCount == 0)
                        {
                            break;
                        }
                        //var forCount = blockCount * 2;
                        commandMethodReturn.Arrays = ReadBulkStrings(blockCount);
                        //var data = Read2NextRN(forCount, -1);
                        //commandMethodReturn.Arrays = data;

                        #region 性能优化前

                        //var wholeBlock = new StringBuilder();
                        //for (int i = 0; i < forCount; i++)
                        //{
                        //    var line = Read2NextRN();
                        //    if (Convert.ToBoolean(i & 1))
                        //    {
                        //        wholeBlock.AppendLine(line);
                        //    }

                        //}
                        //rtn.Data = wholeBlock.ToString();

                        #endregion

                        break;
                    default:
                        commandMethodReturn.Error = "不支持的响应头标记";
                        break;
                }
                rtn.Add(commandMethodReturn);
            }

            _commandStopWatch.Stop();
            rtn.ForEach(r => r.ExecuteTime = _commandStopWatch.Elapsed.TotalSeconds);

            _mutex.ReleaseMutex();

            return rtn;
        }

        /// <summary>
        /// 读取块数据
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        private string ReadBulkStrings(int times)
        {
            //var regexInt = new Regex(@"\d");
            var sb = new StringBuilder();
            for (int i = 0; i < times; i++)
            {
                var head = ReadString2NextRN();
                sb.AppendLine(head);
                //var length = int.Parse(regexInt.Match(head).Value)+2;  //+2意在读取最后的\r\n
                //var length = int.Parse(new string(head.Skip(1).ToArray()))+2;  //+2意在读取最后的\r\n
                var length = int.Parse(head.Substring(1, head.Length - 1)) + 2;  //+2意在读取最后的\r\n
                sb.Append(ReadSpecialBytes(length));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 读取指定的字节数
        /// </summary>
        /// <param name="bytesCount"></param>
        /// <returns></returns>
        private string ReadSpecialBytes(int bytesCount)
        {
            var buffer = new byte[bytesCount];
            _tcpClient.Client.Receive(buffer);
            var stringRead = Encoding.UTF8.GetString(buffer);
            return stringRead;
        }

        /// <summary>
        /// 读取到下一个响应结束标记（\r\n）
        /// </summary>
        /// <returns></returns>
        private string ReadString2NextRN()
        {
            var buffer = new byte[1];
            var byteList = new List<byte>();
            byte lastBuffer = 0;
            while (true)
            {
                _tcpClient.Client.Receive(buffer);
                var b = buffer[0];
                byteList.Add(b);

                if (lastBuffer == 13 && buffer[0] == 10)
                {
                    break;
                }
                lastBuffer = b;
            }
            var stringRead = Encoding.UTF8.GetString(byteList.ToArray());
            //byteList.RemoveRange(byteList.Count - 2, 2);
            return stringRead.TrimEnd('\n', '\r');
        }

        /// <summary>
        /// 读取到指定次数的响应结束标记后返回（\r\n）
        /// <param name="times">次数</param>
        /// <param name="skipLine">-1不跳过，0跳过偶数行，1跳过奇数行</param>
        /// </summary>
        /// <returns></returns>
        private string Read2NextRN(int times, int skipLine)
        {
            var buffer = new byte[1];
            var byteList = new List<byte>();
            byte lastBuffer = 0;
            var readTimes = 0;
            var lineNumber = 1;

            if (skipLine != -1)
            {
                while (true)
                {
                    _tcpClient.Client.Receive(buffer);
                    var b = buffer[0];
                    if ((Convert.ToBoolean(lineNumber & 1) //奇数行
                        &&
                        skipLine == 0)  //不跳过奇数行
                        ||
                        (!Convert.ToBoolean(lineNumber & 1) //偶数行
                        &&
                        skipLine == 1)) //不跳过偶数行
                    {
                        byteList.Add(b);
                    }

                    if (lastBuffer == 13 && buffer[0] == 10)
                    {
                        lineNumber++;
                        if (++readTimes == times)
                        {
                            break;
                        }
                    }
                    lastBuffer = b;
                }
            }
            else
            {
                while (true)
                {
                    _tcpClient.Client.Receive(buffer);
                    var b = buffer[0];
                    byteList.Add(b);

                    if (lastBuffer == 13 && buffer[0] == 10)
                    {
                        if (++readTimes == times)
                        {
                            break;
                        }
                    }
                    lastBuffer = b;
                }
            }

            //byteList.RemoveRange(byteList.Count-2,2);
            var stringRead = Encoding.UTF8.GetString(byteList.ToArray());
            return stringRead.TrimEnd('\n', '\r');
        }

        #region 连接保障

        /// <summary>
        /// 通过心跳机制保证连接稳定
        /// </summary>
        private void KeepConnection()
        {
            Action<object> act = o =>
            {
                while (!_disposeStarted)  //如果未开始释放
                {
                    if (!IsConnectionAlive())  //如果链接死亡
                    {
                        Reconnect();  //重连
                    }
                    Thread.Sleep(HeartBeatInterval);
                }
            };

            ThreadPool.QueueUserWorkItem(new WaitCallback(act));
        }

        /// <summary>
        /// 判断TCP连接是否存活
        /// </summary>
        /// <returns></returns>
        private bool IsConnectionAlive()
        {
            try
            {
                var res = Command("ping");  //尝试与Redis服务器通信
                Debug.Print(string.Concat(res.First().SimpleString," ", DateTime.Now.ToString("O")));
                return string.Equals(res.First().SimpleString.Trim(), "pong", StringComparison.CurrentCultureIgnoreCase);  //判断通信结果
            }
            catch (Exception)
            {
                Debug.Print("lost connection");
                return false;
            }
        }

        /// <summary>
        /// 重连
        /// </summary>
        private void Reconnect()
        {
            try
            {
                _tcpClient.Close();  //尝试关闭原连接
            }
            catch
            {
                // ignored
            }
            InitTcpClient();  //初始化新连接

            Debug.Print("reconnected");
        }

        #endregion

        /// <summary>
        /// 不用了务必及时调用
        /// </summary>
        public void Dispose()
        {
            try
            {
                Command("quit");  //通知Redis关闭连接
                _disposeStarted = true;  //发送释放信号
                _tcpClient.Client.Disconnect(false);  //断开连接
                _tcpClient.Client.Dispose();  //释放socket资源
                _tcpClient.Close();  //释放tcp资源
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    public class CommandMethodReturn
    {
        /// <summary>
        /// +
        /// </summary>
        public string SimpleString { get; set; }
        /// <summary>
        /// -
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// :
        /// </summary>
        public int Integer { get; set; }
        /// <summary>
        /// $
        /// </summary>
        public string BulkStrings { get; set; }
        /// <summary>
        /// *
        /// </summary>
        public string Arrays { get; set; }
        public double ExecuteTime { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("+SimpleString：");
            sb.AppendLine(SimpleString);
            sb.Append("-Error：");
            sb.AppendLine(Error);
            sb.Append(":Integer：");
            sb.AppendLine(Integer.ToString());
            sb.Append("$BulkStrings：");
            sb.AppendLine(BulkStrings);
            sb.Append("*Arrays：");
            sb.AppendLine(Arrays);
            sb.Append("ExecuteTime(ms)：");
            sb.AppendLine((ExecuteTime * 1000).ToString("F7"));

            return sb.ToString();
        }
    }
}

