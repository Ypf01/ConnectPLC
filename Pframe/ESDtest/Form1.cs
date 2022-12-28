using NodeSettings.Node.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NodeSettings;

namespace ESDtest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }
        List<NodeSk> list;
        private void Form1_Load(object sender, EventArgs e)
        {
            //List<NodeESD> list = ESDCFG.LoadXmlFile(Environment.CurrentDirectory+"//121//test.xml");
            list = SKCFG.LoadXmlFile(Environment.CurrentDirectory + "//121//settings.xml");
            //
            list.ForEach(c => c.AlarmEvent += C_AlarmEvent);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            list.ForEach(c =>
            {
                //报警委托如果在开始的时候关联,则开始一次关联一次；
                //解决对策 方法一：加载窗体的时候关联一次  、方法二：开始的时候关联结束的时候取消关联
                //c.AlarmEvent += C_AlarmEvent;
                if (c.IsActive)
                    c.Start();
            });
        }

        private void C_AlarmEvent(object arg1, AlarmEventArgs arg2)
        {
            Invoke(new Action(() =>
            {
                this.textBox1.AppendText(arg2.alarmInfo + "    " + (arg2.IsACK ? "激活" : "取消") + Environment.NewLine);
            }));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            list.ForEach(C =>
            {
                C.Stop();
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //foreach (var item in list)
            //{
            //    if (item.IsActive)
            //        Invoke(new Action(() => { this.textBox1.AppendText(item.Write(code.Text.Trim()).Message + Environment.NewLine); }));
            //}
            string dd = list.Find(c => c.Name.Equals("10#称重系统"))["10#_Counter"].ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //foreach (var item in list)
            //{
            NodeSk sk = list.Find(c => c.Name.Equals("10#称重系统"));
            this.label1.Text = sk.Name + (sk.IsConnected ? "在线" : "离线");
            //}
            //foreach (Control item in panel1.Controls)
            //{
            //    if (list[1].CurrentValue.Keys.Contains(item.Tag.ToString()))
            //        item.Text = list[1].CurrentValue[item.Tag.ToString()].ToString();
            //}
            str = "";
            foreach (string item in sk.CurrentValue.Keys)
            {

                str += item + ":" + sk.CurrentValue[item].ToString() + Environment.NewLine;
            }
            this.label2.Text = str;
        }
        string str = "";
        private void button4_Click(object sender, EventArgs e)
        {
            foreach (var item in list)
            {
                if (item.IsActive)
                {
                    str = "";
                    foreach (var item1 in item.CurrentValue.Keys)
                        str += item1 + ":" + item.CurrentValue[item1].ToString() + "    ";
                    MessageBox.Show(str.Trim());
                }
            }
        }
    }
}
