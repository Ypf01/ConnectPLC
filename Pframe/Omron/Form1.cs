using NodeSettings;
using NodeSettings.Node.Device;
using Pframe;
using Pframe.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Omron
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cip = OmronCIPCFG.LoadXmlFile(Environment.CurrentDirectory + "\\setting\\settings.xml");
            this.cmb_type.DataSource = Enum.GetNames(typeof(VarType));
        }

        List<NodeOmronCIP> cip = null;
        List<string> varName = new List<string>();
        private void btn_Start_Click(object sender, EventArgs e)
        {
            if (cip.Count > 0)
                new Action(() =>
                {
                    varName = null;
                    cip.ForEach(c =>
                    {
                        varName = null;
                        if (c.IsActive)
                        {
                            c.Start();
                            Thread.Sleep(1000);
                            varName = new List<string>(c.CurrentValue.Keys.ToList<string>());
                            if (varName != null)
                                Invoke(new Action(() =>
                                {
                                    this.cmd_varname.DataSource = varName;
                                }));
                        }
                    });
                }).BeginInvoke(null, null);
        }

        private void cmd_varname_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (varName != null && this.cmd_varname.SelectedItem != null)
            {
                if (cip[0].IsConnected && cip[0].CurrentValue.Keys.Contains(this.cmd_varname.SelectedItem.ToString()))
                    this.lb_Result.Text = cip[0].CurrentValue[this.cmd_varname.SelectedItem.ToString()].ToString();
            }
        }

        private void btn_Write_Click(object sender, EventArgs e)
        {
            CalResult RR;
            if (cip[0].IsConnected)
            {
                if (this.txt_name.Text.Trim().Length == 0)
                    RR = cip[0].Write(this.cmd_varname.Text, this.txt_value.Text);
                else
                    cip[0].Write(this.txt_name.Text.Trim(), this.txt_value.Text.Trim(), (VarType)Enum.Parse(typeof(VarType), this.cmb_type.Text.Trim()));
            }
        }

        private void btn_Read_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string dd = string.Empty;
            cip[0].CurrentValue.Keys.ToList().ForEach(c =>
            {
                dd += c + ":" + cip[0].CurrentValue[c].ToString() + Environment.NewLine;
            });
            MessageBox.Show(dd.Trim());
        }
    }
}
