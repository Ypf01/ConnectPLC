using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pframe;
using NodeSettings;
using NodeSettings.Node.Device;
using Pframe.Common;
using NodeSettings.Node.Variable;

namespace PframeTest
{
    public partial class Form1 : Form
    {
        List<NodeInovance> melsec;
        public Form1()
        {
            InitializeComponent();
            this.cmd_melsec.DataSource = Enum.GetNames(typeof(DataType));
            melsec = InovanceCFG.LoadXmlFile(Environment.CurrentDirectory + "//Setting//2.xml");

            melsec[0].AlarmEvent += Form1_AlarmEvent;

        }

        private void Form1_AlarmEvent(object arg1, AlarmEventArgs arg2)
        {
            this.Invoke(new Action(() =>
            {
                this.textBox1.Text = (arg2.IsACK ? "报警激活" : "报警取消") + "    " + arg2.alarmInfo + Environment.NewLine + this.textBox1.Text;
            }));
            //DialogResult result = arg2.IsACK ? MessageBox.Show(arg2.alarmInfo) : MessageBox.Show("报警被取消" + arg2.alarmInfo);
        }

        private void btn_Write_Click(object sender, EventArgs e)
        {
            if (melsec[0].IsConnected && this.txt_Value.Text.Trim().ToLower().Equals("close"))
                melsec[0].inovance.DisConnect();
            DataType datatype = (DataType)Enum.Parse(typeof(DataType), this.cmd_melsec.Text.Trim(), true);
            try
            {
                melsec[0].Write(this.txt_Adress.Text.Trim(), this.txt_Value.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_Read_Click(object sender, EventArgs e)
        {
            object result = 0;
            object finaly = 0;
            DataType datatype = (DataType)Enum.Parse(typeof(DataType), this.cmd_melsec.Text.Trim(), true);
            switch (datatype)
            {
                case DataType.Bool:
                    bool result1 = false;
                    result = melsec[0].inovance.ReadBool(this.txt_Adress.Text.Trim(), ref result1);
                    finaly = result1;
                    break;
                case DataType.Byte:
                    byte result2 = 0;
                    melsec[0].inovance.ReadByte(this.txt_Adress.Text.Trim(), ref result2);
                    finaly = result2;
                    break;
                case DataType.Short:
                    short result3 = 0;
                    result = melsec[0].inovance.ReadShort(this.txt_Adress.Text.Trim(), ref result3);
                    finaly = result3;
                    break;
                case DataType.UShort:
                    ushort result4 = 0;
                    result = melsec[0].inovance.ReadUshort(this.txt_Adress.Text.Trim(), ref result4);
                    finaly = result4;
                    break;
                case DataType.Int:
                    int result5 = 0;
                    result = melsec[0].inovance.ReadInt(this.txt_Adress.Text.Trim(), ref result5);
                    finaly = result5;
                    break;
                case DataType.UInt:
                    uint result6 = 0;
                    result = melsec[0].inovance.ReadUInt(this.txt_Adress.Text.Trim(), ref result6);
                    finaly = result6;
                    break;
                case DataType.Float:
                    float result7 = 0;
                    result = melsec[0].inovance.ReadFloat(this.txt_Adress.Text.Trim(), ref result7);
                    finaly = result7;
                    break;
                case DataType.Double:
                    double result8 = 0;
                    result = melsec[0].inovance.ReadDouble(this.txt_Adress.Text.Trim(), ref result8);
                    finaly = result8;
                    break;
                case DataType.Long:
                    break;
                case DataType.ULong:
                    break;
                case DataType.String:
                    string result9 = "";
                    melsec[0].inovance.ReadString(this.txt_Adress.Text.Trim(), Convert.ToInt16(this.txt_Value.Text), ref result9);
                    finaly = result9;
                    break;
                case DataType.ByteArray:
                    break;
                case DataType.HexString:
                    break;
                default:
                    break;
            }
            this.lb_Result.Text = finaly.ToString();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            melsec[0].Start();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (melsec != null && melsec.Count > 0)
            {
                melsec.ForEach(c =>
                {
                    c.Stop();
                });
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (melsec != null && melsec.Count > 0)
            {
                if (melsec[0].IsConnected)
                {
                    d0.Text = melsec[0].GetValue<string>("HeartBeat");
                    d1.Text = melsec[0].GetValue<string>("Status");
                    d2.Text = melsec[0].GetValue<string>("LeftResult");
                    d3.Text = melsec[0].GetValue<string>("LeftCamera");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            object KL = melsec[0].GetValue<int>("D2");
            KL = melsec[0].GetValue("D2");
            KL = melsec[0].GetVariableNode("D2");
        }
    }
}
