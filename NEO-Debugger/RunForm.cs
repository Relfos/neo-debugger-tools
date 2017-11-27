using LunarParser;
using LunarParser.JSON;
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

        public static byte[] HexToByte(string HexString)
        {
            if (HexString.Length % 2 != 0)
                throw new Exception("Invalid HEX");
            byte[] retArray = new byte[HexString.Length / 2];
            for (int i = 0; i < retArray.Length; ++i)
            {
                retArray[i] = byte.Parse(HexString.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return retArray;
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
                return HexToByte(item.Value.Substring(2));
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

                    if (lastParams != null && entry.Key.Equals(lastParams))
                    {
                        paramsList.SelectedIndex = paramsList.Items.Count - 1;
                    }
                }
            }

        }
    }
}
