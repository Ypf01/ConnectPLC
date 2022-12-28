using NodeSettings;
using NodeSettings.Node.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PframeTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        List<NodeInovance> ino;
        private void button1_Click(object sender, EventArgs e)
        {
            ino = InovanceCFG.LoadXmlFile(Environment.CurrentDirectory + "//Setting//2.xml");
            ino[0].Start();
            ino[0].Write("组对象变量", "0");
        }
    }
}
