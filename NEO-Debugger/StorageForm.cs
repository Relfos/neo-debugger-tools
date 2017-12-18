using Neo.Emulator.API;
using Neo.Debugger.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neo.Tools.AVM;

namespace Neo.Debugger
{
    public partial class StorageForm : Form
    {
        public StorageForm()
        {
            InitializeComponent();
        }

        private void StorageForm_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            dataGridView1.Columns.Add("Key", "Key");
            dataGridView1.Columns.Add("Values", "Content");

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[0].FillWeight = 3;

            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].FillWeight = 4;

            foreach (var entry in Storage.storage)
            {
                dataGridView1.Rows.Add(FormattingUtils.OutputData(entry.Key, false), FormattingUtils.OutputData(entry.Value, false));
            }
        }
    }
}
