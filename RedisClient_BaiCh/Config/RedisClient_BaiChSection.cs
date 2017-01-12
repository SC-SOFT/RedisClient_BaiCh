using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RedisClient_BaiCh.Config
{
    public class RedisClient_BaiChSection : ConfigurationSection
    {
        [ConfigurationProperty("Connection")]
        public ConnectionElement Connection
        {
            get { return this["Connection"] as ConnectionElement; }
            set { this["Connection"] = value; }
        }
    }
}
