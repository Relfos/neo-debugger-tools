using LunarParser;
using LunarParser.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Neo.Debugger
{
    public partial class RunForm : Form
    {
        public NeoDebugger debugger;

        public RunForm()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = paramsList.Text;
            if (_paramMap.ContainsKey(key))
            {
                var node = _paramMap[key];

                var json = JSONWriter.WriteToString(node);
                textBox1.Text = json;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var json = textBox1.Text;

            if (string.IsNullOrEmpty(json))
            {
                MessageBox.Show("Invalid input!");
                return;
            }

            DataNode node;

            try
            {
                node = JSONReader.ReadFromString(json);
            }
            catch
            {
                MessageBox.Show("Error parsing input!");
                return;
            }

            var items = node.GetNode("params");

            debugger.ContractArgs.Clear();
            foreach (var item in items.Children)
            {
                // TODO - auto convert value to proper types, currently everything is assumed to be strings!

                var obj = ConvertArgument(item);
                debugger.ContractArgs.Add(obj);
            }
        }

        private object ConvertArgument(DataNode item)
        {
            if (item.HasChildren)
            {
                var list = new List<object>();
                foreach (var child in item.Children)
                {
                    list.Add(ConvertArgument(child));
                }
                return list;
            }

            int intVal;
            if (int.TryParse(item.Value, out intVal))
            {
                return intVal;
            }
            else
            {
                return item.Value;
            }
        }

        private static Dictionary<string, DataNode> _paramMap = null;

        private void RunForm_Shown(object sender, EventArgs e)
        { 
            if (!string.IsNullOrEmpty(MainForm.targetAVMPath))
            {
                var fileName = MainForm.targetAVMPath.Replace(".avm", ".json");
                if (File.Exists(fileName))
                {
                    try
                    {
                        _paramMap = new Dictionary<string, DataNode>();

                        var contents = File.ReadAllText(fileName);

                        var contractInfo = JSONReader.ReadFromString(contents);

                        var contractNode = contractInfo["contract"];
                        var inputs = contractNode["inputs"];

                        paramsList.Items.Clear();
                        foreach (var node in inputs.Children)
                        {
                            var name = node.GetString("name");
                            var data = node.GetNode("params");
                            _paramMap[name] = data;
                        }

                        paramsList.SelectedIndex = 0;
                    }
                    catch
                    {
                    }                    
                }
            }

            button1.Enabled = _paramMap != null && _paramMap.Count > 0 ;
            paramsList.Items.Clear();

            if (_paramMap != null)
            {
                foreach (var entry in _paramMap)
                {
                    paramsList.Items.Add(entry.Key);
                }
            }

        }
    }
}
