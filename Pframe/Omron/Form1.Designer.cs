namespace Omron
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
            this.btn_Start = new System.Windows.Forms.Button();
            this.btn_Read = new System.Windows.Forms.Button();
            this.btn_Write = new System.Windows.Forms.Button();
            this.txt_value = new System.Windows.Forms.TextBox();
            this.cmd_varname = new System.Windows.Forms.ComboBox();
            this.lb_Result = new System.Windows.Forms.Label();
            this.cmb_type = new System.Windows.Forms.ComboBox();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(106, 29);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(75, 23);
            this.btn_Start.TabIndex = 0;
            this.btn_Start.Text = "连接";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // btn_Read
            // 
            this.btn_Read.Location = new System.Drawing.Point(411, 79);
            this.btn_Read.Name = "btn_Read";
            this.btn_Read.Size = new System.Drawing.Size(75, 23);
            this.btn_Read.TabIndex = 1;
            this.btn_Read.Text = "读取";
            this.btn_Read.UseVisualStyleBackColor = true;
            this.btn_Read.Click += new System.EventHandler(this.btn_Read_Click);
            // 
            // btn_Write
            // 
            this.btn_Write.Location = new System.Drawing.Point(411, 29);
            this.btn_Write.Name = "btn_Write";
            this.btn_Write.Size = new System.Drawing.Size(75, 23);
            this.btn_Write.TabIndex = 2;
            this.btn_Write.Text = "写入";
            this.btn_Write.UseVisualStyleBackColor = true;
            this.btn_Write.Click += new System.EventHandler(this.btn_Write_Click);
            // 
            // txt_value
            // 
            this.txt_value.Location = new System.Drawing.Point(245, 29);
            this.txt_value.Name = "txt_value";
            this.txt_value.Size = new System.Drawing.Size(100, 21);
            this.txt_value.TabIndex = 3;
            // 
            // cmd_varname
            // 
            this.cmd_varname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmd_varname.FormattingEnabled = true;
            this.cmd_varname.Location = new System.Drawing.Point(595, 29);
            this.cmd_varname.Name = "cmd_varname";
            this.cmd_varname.Size = new System.Drawing.Size(121, 20);
            this.cmd_varname.TabIndex = 4;
            this.cmd_varname.SelectedIndexChanged += new System.EventHandler(this.cmd_varname_SelectedIndexChanged);
            // 
            // lb_Result
            // 
            this.lb_Result.AutoSize = true;
            this.lb_Result.Location = new System.Drawing.Point(637, 79);
            this.lb_Result.Name = "lb_Result";
            this.lb_Result.Size = new System.Drawing.Size(53, 12);
            this.lb_Result.TabIndex = 5;
            this.lb_Result.Text = "测试结果";
            // 
            // cmb_type
            // 
            this.cmb_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_type.FormattingEnabled = true;
            this.cmb_type.Location = new System.Drawing.Point(126, 258);
            this.cmb_type.Name = "cmb_type";
            this.cmb_type.Size = new System.Drawing.Size(90, 20);
            this.cmb_type.TabIndex = 6;
            // 
            // txt_name
            // 
            this.txt_name.Location = new System.Drawing.Point(259, 258);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(100, 21);
            this.txt_name.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(595, 190);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 340);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txt_name);
            this.Controls.Add(this.cmb_type);
            this.Controls.Add(this.lb_Result);
            this.Controls.Add(this.cmd_varname);
            this.Controls.Add(this.txt_value);
            this.Controls.Add(this.btn_Write);
            this.Controls.Add(this.btn_Read);
            this.Controls.Add(this.btn_Start);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Button btn_Read;
        private System.Windows.Forms.Button btn_Write;
        private System.Windows.Forms.TextBox txt_value;
        private System.Windows.Forms.ComboBox cmd_varname;
        private System.Windows.Forms.Label lb_Result;
        private System.Windows.Forms.ComboBox cmb_type;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.Button button1;
    }
}

