using LunarParser;
using LunarParser.JSON;
using Neo.Debugger.Utils;
using Neo.Emulator.API;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace Neo.Debugger
{
    public partial class RunForm : Form
    {
        public NeoDebugger debugger;

        private string lastParams = null;

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
                contractInputField.Text = json;
            }

            lastParams = key;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var json = contractInputField.Text;

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

            BigInteger intVal;

            if (item.Kind == NodeKind.Numeric)
            {                
                if (BigInteger.TryParse(item.Value, out intVal))
                {
                    return intVal;
                }                
                else
                {
                    return 0;
                }
            }
            else
            if (item.Kind == NodeKind.Boolean)
            {
                return "true".Equals(item.Value.ToLowerInvariant()) ? true : false;
            }
            else
            if (item.Kind == NodeKind.Null)
            {
                return null;
            }
            else
            if (item.Value.StartsWith("0x"))
            {
                return item.Value.Substring(2).HexToByte();
            }
            else
            {
                return item.Value;
            }
        }

        private static Dictionary<string, DataNode> _paramMap = null;

        private void RunForm_Shown(object sender, EventArgs e)
        {
            if (Runtime.invokerKeys == null && File.Exists("last.key"))
            {
                var privKey = File.ReadAllBytes("last.key");
                if (privKey.Length == 32)
                {
                    Runtime.invokerKeys = new NeoLux.KeyPair(privKey);
                }
            }

            if (Runtime.invokerKeys != null)
            {
                addressLabel.Text = Runtime.invokerKeys.address;
            }
            else
            {
                addressLabel.Text = "(No key loaded)";
            }

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

                    if (lastParams != null && entry.Key.Equals(lastParams))
                    {
                        paramsList.SelectedIndex = paramsList.Items.Count - 1;
                    }
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string input = "";
            if (InputUtils.ShowInputDialog("Enter private key", ref input) == DialogResult.OK)
            {
                var privKey = input.HexToByte();
                if (privKey.Length == 32)
                {
                    Runtime.invokerKeys = new NeoLux.KeyPair(privKey);
                    addressLabel.Text = Runtime.invokerKeys.address;

                    File.WriteAllBytes("last.key", privKey);
                }
                else
                {
                    MessageBox.Show("Invalid private key, length should be 32");
                }
            }
        }

    }
}
