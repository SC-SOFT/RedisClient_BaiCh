using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace RedisClient_BaiCh
{
    public class RedisCmdReturn
    {
        /// <summary>
        /// 执行结果，OK无异常，否则为异常消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 远程执行时间（命令往返）
        /// </summary>
        public double RemoteExecuteTime { get; set; }

        public RedisCmdReturn(CommandMethodReturn commandMethodReturn)
        {
            Msg = string.IsNullOrEmpty(commandMethodReturn.Error) ? "OK" : commandMethodReturn.Error;
            RemoteExecuteTime = commandMethodReturn.ExecuteTime;
        }
    }
}
