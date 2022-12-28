namespace PframeTest
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
            this.components = new System.ComponentModel.Container();
            this.d0 = new System.Windows.Forms.Label();
            this.txt_Adress = new System.Windows.Forms.TextBox();
            this.btn_Write = new System.Windows.Forms.Button();
            this.btn_Read = new System.Windows.Forms.Button();
            this.txt_Value = new System.Windows.Forms.TextBox();
            this.cmd_melsec = new System.Windows.Forms.ComboBox();
            this.btn_start = new System.Windows.Forms.Button();
            this.d1 = new System.Windows.Forms.Label();
            this.d2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.d3 = new System.Windows.Forms.Label();
            this.lb_Result = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // d0
            // 
            this.d0.AutoSize = true;
            this.d0.Location = new System.Drawing.Point(213, 204);
            this.d0.Name = "d0";
            this.d0.Size = new System.Drawing.Size(41, 12);
            this.d0.TabIndex = 13;
            this.d0.Text = "label4";
            // 
            // txt_Adress
            // 
            this.txt_Adress.Location = new System.Drawing.Point(285, 81);
            this.txt_Adress.Name = "txt_Adress";
            this.txt_Adress.Size = new System.Drawing.Size(100, 21);
            this.txt_Adress.TabIndex = 12;
            // 
            // btn_Write
            // 
            this.btn_Write.Location = new System.Drawing.Point(341, 137);
            this.btn_Write.Name = "btn_Write";
            this.btn_Write.Size = new System.Drawing.Size(75, 23);
            this.btn_Write.TabIndex = 11;
            this.btn_Write.Text = "写入";
            this.btn_Write.UseVisualStyleBackColor = true;
            this.btn_Write.Click += new System.EventHandler(this.btn_Write_Click);
            // 
            // btn_Read
            // 
            this.btn_Read.Location = new System.Drawing.Point(180, 138);
            this.btn_Read.Name = "btn_Read";
            this.btn_Read.Size = new System.Drawing.Size(75, 23);
            this.btn_Read.TabIndex = 10;
            this.btn_Read.Text = "读取";
            this.btn_Read.UseVisualStyleBackColor = true;
            this.btn_Read.Click += new System.EventHandler(this.btn_Read_Click);
            // 
            // txt_Value
            // 
            this.txt_Value.Location = new System.Drawing.Point(436, 83);
            this.txt_Value.Name = "txt_Value";
            this.txt_Value.Size = new System.Drawing.Size(100, 21);
            this.txt_Value.TabIndex = 9;
            // 
            // cmd_melsec
            // 
            this.cmd_melsec.FormattingEnabled = true;
            this.cmd_melsec.Location = new System.Drawing.Point(109, 83);
            this.cmd_melsec.Name = "cmd_melsec";
            this.cmd_melsec.Size = new System.Drawing.Size(121, 20);
            this.cmd_melsec.TabIndex = 8;
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(549, 13);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 23);
            this.btn_start.TabIndex = 14;
            this.btn_start.Text = "开始";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // d1
            // 
            this.d1.AutoSize = true;
            this.d1.Location = new System.Drawing.Point(283, 204);
            this.d1.Name = "d1";
            this.d1.Size = new System.Drawing.Size(41, 12);
            this.d1.TabIndex = 15;
            this.d1.Text = "label4";
            // 
            // d2
            // 
            this.d2.AutoSize = true;
            this.d2.Location = new System.Drawing.Point(344, 204);
            this.d2.Name = "d2";
            this.d2.Size = new System.Drawing.Size(41, 12);
            this.d2.TabIndex = 16;
            this.d2.Text = "label4";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // d3
            // 
            this.d3.AutoSize = true;
            this.d3.Location = new System.Drawing.Point(409, 204);
            this.d3.Name = "d3";
            this.d3.Size = new System.Drawing.Size(41, 12);
            this.d3.TabIndex = 17;
            this.d3.Text = "label4";
            // 
            // lb_Result
            // 
            this.lb_Result.AutoSize = true;
            this.lb_Result.Location = new System.Drawing.Point(527, 137);
            this.lb_Result.Name = "lb_Result";
            this.lb_Result.Size = new System.Drawing.Size(41, 12);
            this.lb_Result.TabIndex = 18;
            this.lb_Result.Text = "label4";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(248, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 19;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(156, 298);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(556, 192);
            this.textBox1.TabIndex = 20;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 502);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lb_Result);
            this.Controls.Add(this.d3);
            this.Controls.Add(this.d2);
            this.Controls.Add(this.d1);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.d0);
            this.Controls.Add(this.txt_Adress);
            this.Controls.Add(this.btn_Write);
            this.Controls.Add(this.btn_Read);
            this.Controls.Add(this.txt_Value);
            this.Controls.Add(this.cmd_melsec);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label d0;
        private System.Windows.Forms.TextBox txt_Adress;
        private System.Windows.Forms.Button btn_Write;
        private System.Windows.Forms.Button btn_Read;
        private System.Windows.Forms.TextBox txt_Value;
        private System.Windows.Forms.ComboBox cmd_melsec;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Label d1;
        private System.Windows.Forms.Label d2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label d3;
        private System.Windows.Forms.Label lb_Result;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
    }
}

