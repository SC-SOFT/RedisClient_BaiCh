using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace RedisClient_BaiCh
{
    public class RedisCmdReturnAuth : RedisCmdReturn
    {
        public RedisCmdReturnAuth(CommandMethodReturn commandMethodReturn) : base(commandMethodReturn)
        {
            
        }
    }
}
