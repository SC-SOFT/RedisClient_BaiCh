namespace TesterWinform
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.性能测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.次插入ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.次读取ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.次管道插入ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.次管道读取ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(416, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 64);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 28);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(398, 93);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(12, 127);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox2.Size = new System.Drawing.Size(479, 243);
            this.textBox2.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(416, 98);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.性能测试ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(503, 25);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 性能测试ToolStripMenuItem
            // 
            this.性能测试ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.次插入ToolStripMenuItem,
            this.次读取ToolStripMenuItem,
            this.次管道插入ToolStripMenuItem,
            this.次管道读取ToolStripMenuItem});
            this.性能测试ToolStripMenuItem.Name = "性能测试ToolStripMenuItem";
            this.性能测试ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.性能测试ToolStripMenuItem.Text = "性能测试";
            // 
            // 次插入ToolStripMenuItem
            // 
            this.次插入ToolStripMenuItem.Name = "次插入ToolStripMenuItem";
            this.次插入ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.次插入ToolStripMenuItem.Text = "1W次独立插入";
            this.次插入ToolStripMenuItem.Click += new System.EventHandler(this.次插入ToolStripMenuItem_Click);
            // 
            // 次读取ToolStripMenuItem
            // 
            this.次读取ToolStripMenuItem.Name = "次读取ToolStripMenuItem";
            this.次读取ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.次读取ToolStripMenuItem.Text = "1W次独立读取";
            this.次读取ToolStripMenuItem.Click += new System.EventHandler(this.次读取ToolStripMenuItem_Click);
            // 
            // 次管道插入ToolStripMenuItem
            // 
            this.次管道插入ToolStripMenuItem.Name = "次管道插入ToolStripMenuItem";
            this.次管道插入ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.次管道插入ToolStripMenuItem.Text = "10W次管道插入";
            this.次管道插入ToolStripMenuItem.Click += new System.EventHandler(this.次管道插入ToolStripMenuItem_Click);
            // 
            // 次管道读取ToolStripMenuItem
            // 
            this.次管道读取ToolStripMenuItem.Name = "次管道读取ToolStripMenuItem";
            this.次管道读取ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.次管道读取ToolStripMenuItem.Text = "10W次管道读取";
            this.次管道读取ToolStripMenuItem.Click += new System.EventHandler(this.次管道读取ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 382);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 性能测试ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 次插入ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 次读取ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 次管道插入ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 次管道读取ToolStripMenuItem;
    }
}

