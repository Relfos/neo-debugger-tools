using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neo.Debugger.Forms
{
    public partial class CSharpCompilerForm : Form
    {
        public CSharpCompilerForm()
        {
            InitializeComponent();
        }

        private void CSharpCompilerForm_Shown(object sender, EventArgs e)
        {
            debugBtn.Enabled = false;
            listBox1.Items.Clear();
            listBox1.Visible = false;
            this.Height -= listBox1.Height;
        }

        private void CSharpCompilerForm_Load(object sender, EventArgs e)
        {
        }

        private void compileBtn_Click(object sender, EventArgs e)
        {
            listBox1.Visible = true;
            this.Height += listBox1.Height;

            var log = new List<string>();
            //var assembly = CSharpCompiler.Compile(textBox1.Text, log);

            listBox1.Items.Clear();
            foreach (var item in log)
            {
                listBox1.Items.Add(item);
            }
        }
    }
}
