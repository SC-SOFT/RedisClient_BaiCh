using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        private Mutex _mutex = new Mutex();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serverIp">服务器IP</param>
        /// <param name="serverPort">服务器端口</param>
        /// <param name="auth">服务器密码</param>
        public RedisClient(string serverIp, int serverPort,string auth)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
            Auth = auth;
            InitTcpClient();
            _tcpClient.Connect(serverIp, serverPort);
            AuthAsync().Wait();

            //KeepConnection();
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serverIp">服务器IP</param>
        /// <param name="serverPort">服务器端口</param>
        public RedisClient(string serverIp, int serverPort)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
            InitTcpClient();
            _tcpClient.Connect(serverIp, serverPort);

            //KeepConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitTcpClient()
        {
            _tcpClient = new TcpClient
            {
                SendTimeout = SendTimeout,
                ReceiveTimeout = ReceiveTimeout
            };
        }

        /// <summary>
        /// Auth
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
        /// Set
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<RedisCmdReturnSet> SetAsync(string key, string value)
        {
            var rtn = new Task<RedisCmdReturnSet>(delegate
            {

                var sb = new StringBuilder();
                sb.Append("set ");
                sb.Append(key);
                sb.Append(" ");
                sb.Append(value);
                sb.Append("\r\n");
                var res = Command(sb.ToString());
                var redisCmdReturnSet = new RedisCmdReturnSet(res.First());
                return redisCmdReturnSet;
            });
            rtn.Start();
            return rtn;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="key"></param>
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
        /// Quit
        /// </summary>
        /// <returns></returns>
        public Task<RedisCmdReturnQuit> QuitAsync()
        {
            var rtn = new Task<RedisCmdReturnQuit>(delegate
            {

                var res = Command("Quit");
                var redisCmdReturnQuit = new RedisCmdReturnQuit(res.First());
                return redisCmdReturnQuit;
            });
            rtn.Start();
            return rtn;
        }

        /// <summary>
        /// Keys
        /// </summary>
        /// <returns></returns>
        public Task<RedisCmdReturnKeys> Keys()
        {
            var rtn = new Task<RedisCmdReturnKeys>(delegate
            {

                var res = Command("Quit");
                var redisCmdReturnKeys = new RedisCmdReturnKeys(res.First());
                return redisCmdReturnKeys;
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
                        commandMethodReturn.SimpleString = Read2NextRN();
                        break;
                    case "-":  //异常回复
                        commandMethodReturn.Error = Read2NextRN();
                        break;
                    case ":":  //整数回复
                        commandMethodReturn.Integer = int.Parse(Read2NextRN());
                        break;
                    case "$":  //块回复
                        var length = Read2NextRN();
                        if (length == "-1")
                        {
                            break;
                        }
                        var content = Read2NextRN();
                        commandMethodReturn.BulkStrings = content;
                        break;
                    case "*":  //多块回复
                        var blockCount = int.Parse(Read2NextRN());
                        if (blockCount==0)
                        {
                            break;
                        }
                        var forCount = blockCount * 2;
                        var data = Read2NextRN(forCount, -1);
                        commandMethodReturn.Arrays = data;

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
                }
                rtn.Add(commandMethodReturn);
            }

            _commandStopWatch.Stop();
            rtn.ForEach(r => r.ExecuteTime = _commandStopWatch.Elapsed.TotalSeconds);

            _mutex.ReleaseMutex();

            return rtn;
        }

        /// <summary>
        /// 读取到下一个响应结束标记（\r\n）
        /// </summary>
        /// <returns></returns>
        private string Read2NextRN()
        {
            var buffer = new byte[1];
            var byteList = new List<byte>();
            string stringRead;
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
            stringRead = Encoding.UTF8.GetString(byteList.ToArray());
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
            string stringRead;
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
            stringRead = Encoding.UTF8.GetString(byteList.ToArray());
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
                Debug.Print(res.First().SimpleString);
                return string.Equals(res.First().SimpleString.Trim(), "+pong", StringComparison.CurrentCultureIgnoreCase);  //判断通信结果
            }
            catch (Exception)
            {
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
            _tcpClient.Connect(ServerIp, ServerPort);  //重新连接
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
            sb.AppendLine((ExecuteTime*1000).ToString("F7"));

            return sb.ToString();
        }
    }
}

