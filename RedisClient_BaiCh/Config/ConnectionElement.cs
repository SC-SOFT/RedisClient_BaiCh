using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RedisClient_BaiCh.Config
{
    public class ConnectionElement : ConfigurationElement
    {
        [ConfigurationProperty("Server")]
        public string Server
        {
            get
            {
                return this["Server"] as string;
            }

            set
            {
                this["Server"] = value;
            }
        }
        [ConfigurationProperty("Port")]
        public string Port
        {
            get
            {
                return this["Port"] as string;
            }

            set
            {
                this["Port"] = value;
            }
        }
        [ConfigurationProperty("Auth")]
        public string Auth
        {
            get
            {
                return this["Auth"] as string;
            }

            set
            {
                this["Auth"] = value;
            }
        }
    }
}
