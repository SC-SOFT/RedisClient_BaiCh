using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace RedisClient_BaiCh
{
    public class RedisCmdReturnGet: RedisCmdReturn
    {
        /// <summary>
        /// 获取到的数据，空字符串表示指定的键不存在
        /// </summary>
        public string Data { get; set; }

        public RedisCmdReturnGet(CommandMethodReturn commandMethodReturn) : base(commandMethodReturn)
        {
            Data = commandMethodReturn.BulkStrings;
        }
    }
}
