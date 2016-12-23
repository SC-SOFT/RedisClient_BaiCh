using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedisClient_BaiCh;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc=new RedisClient("192.168.25.171",6379);
            //rc.Set("key","123");
            Console.WriteLine(rc.Get("key"));
            Console.ReadLine();
        }
    }
}
