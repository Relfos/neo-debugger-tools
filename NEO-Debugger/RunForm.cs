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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_paramMap == null)
            {
                MessageBox.Show("Parameter map not loaded");
                return;
            }

            if (paramsList.SelectedIndex<0)
            {
                MessageBox.Show("Select an input from the list");
                return;
            }

            var selectedName = paramsList.Items[paramsList.SelectedIndex].ToString();

            if (!_paramMap.ContainsKey(selectedName))
            {
                MessageBox.Show("Invalid function selected!");
                return;
            }

            var items = _paramMap[selectedName];

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
            if (_paramMap == null && !string.IsNullOrEmpty(MainForm.targetAVMPath))
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
            foreach (var entry in _paramMap)
            {
                paramsList.Items.Add(entry.Key);
            }           

        }
    }
}
