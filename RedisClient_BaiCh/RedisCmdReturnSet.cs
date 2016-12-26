using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace RedisClient_BaiCh
{
    public class RedisCmdReturnSet: RedisCmdReturn
    {
        public RedisCmdReturnSet(CommandMethodReturn commandMethodReturn) : base(commandMethodReturn)
        {

        }
    }
}
