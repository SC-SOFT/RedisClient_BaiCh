using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RedisClient_BaiCh;
using RedisClient_BaiCh.CmdReturnEntities;

namespace TesterWinform
{
    public partial class Form1 : Form
    {
        //readonly RedisClient _redisClient = new RedisClient("192.168.25.171", 6379,"7654321");
        readonly RedisClient _redisClient = new RedisClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var rtn = _redisClient.Command(textBox1.Text);
            textBox2.Text = string.Join(Environment.NewLine+Environment.NewLine, rtn.Select(r => r.ToString()));
            Text = rtn.Sum(r=>r.ExecuteTime).ToString("F7");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var rtn = _redisClient.MSetAsync(new [] { "a", "b" },new [] {"1","2"}).Result;
            textBox2.Text = rtn.Msg;
            Text = rtn.RemoteExecuteTime.ToString("f7");

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _redisClient.Dispose();
        }

        private void 次插入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            var taskListSet = new List<Task<RedisCmdReturnSet>>();
            for (int i = 0; i < 10000; i++)
            {
                taskListSet.Add(_redisClient.SetAsync(i.ToString(), i.ToString()));
            }
            Task.WaitAll(taskListSet.ToArray());
            sw.Stop();
            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString("F7"));
        }

        private void 次读取ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            var taskList = new List<Task<RedisCmdReturnGet>>();
            for (int i = 0; i < 10000; i++)
            {
                taskList.Add(_redisClient.GetAsync(i.ToString()));
            }
            Task.WaitAll(taskList.ToArray());
            sw.Stop();
            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString("F7"));
        }

        private void 次管道插入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            var sb = new StringBuilder();
            for (int i = 0; i < 100000; i++)
            {
                sb.Append("set key");
                sb.Append(i);
                sb.Append(" ");
                sb.Append("好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好好");
                sb.Append("\r\n");
            }
            sw.Start();
            _redisClient.Command(sb.ToString());
            sw.Stop();
            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString("F7"));
        }

        private void 次管道读取ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            var sb = new StringBuilder();
            for (int i = 0; i < 100000; i++)
            {
                sb.Append("get key");
                sb.Append(i);
                sb.Append("\r\n");
            }
            sw.Start();
            _redisClient.Command(sb.ToString());
            sw.Stop();
            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString("F7"));
        }
    }
}
