using System;
using System.Collections.Generic;
using System.Linq;

namespace RedisClient_BaiCh.CmdReturnEntities
{
    public class RedisCmdReturnKeys: RedisCmdReturn
    {
        public List<string> Keys { get; set; }

        public RedisCmdReturnKeys(CommandMethodReturn commandMethodReturn) : base(commandMethodReturn)
        {
            Keys = commandMethodReturn.Arrays.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
