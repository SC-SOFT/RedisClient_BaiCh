using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace RedisClient_BaiCh
{
    public class RedisCmdReturnQuit : RedisCmdReturn
    {
        /// <summary>
        /// 服务器响应
        /// </summary>
        public string Data { get; set; }
    }
}
