using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RedisClient_BaiCh;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var count=100000;

            var sb=new StringBuilder();
            for (int i = 0; i < 5000; i++)
            {
                sb.Append("1");
            }
            sb.Append("\r\n");
            var str = sb.ToString();
            var b = Encoding.UTF8.GetBytes(str).ToList();

            var reg=new Regex("\\r\\n$");
            var sw=new Stopwatch();
            while (true)
            {
                sw.Restart();
                for (int i = 0; i < count; i++)
                {
                    var aa = Encoding.UTF8.GetString(b.ToArray()).TrimEnd('\n', '\r');//.TrimEnd('\r');
                }
                Console.WriteLine(sw.Elapsed.TotalSeconds.ToString("F7"));
                sw.Restart();
                for (int i = 0; i < count; i++)
                {
                    var bb = reg.Replace(Encoding.UTF8.GetString(b.ToArray()), string.Empty);
                }
                Console.WriteLine(sw.Elapsed.TotalSeconds.ToString("F7"));
                //sw.Restart();
                //for (int i = 0; i < count; i++)
                //{
                //    var dd = Encoding.UTF8.GetString(b.Take(b.Count - 2).ToArray());
                //}
                //Console.WriteLine(sw.Elapsed.TotalSeconds.ToString("F7"));
                sw.Restart();
                for (int i = 0; i < count; i++)
                {
                    sw.Stop();
                    b = Encoding.UTF8.GetBytes(str).ToList();
                    sw.Start();
                    b.RemoveRange(b.Count - 2, 2);
                    var cc = Encoding.UTF8.GetString(b.ToArray());
                }
                Console.WriteLine(sw.Elapsed.TotalSeconds.ToString("F7"));
                sw.Stop();

                Console.ReadLine();
                Console.Clear();
            }


        }
    }
}
