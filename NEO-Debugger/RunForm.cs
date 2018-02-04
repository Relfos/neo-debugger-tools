using LunarParser;
using LunarParser.JSON;
using Neo.Debugger.Utils;
using Neo.Emulator;
using Neo.Emulator.API;
using Neo.Emulator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace Neo.Debugger
{
    public partial class RunForm : Form
    {
        public NeoEmulator emulator;

        private string lastParams = null;

        public RunForm()
        {
            InitializeComponent();

            assetListBox.Items.Clear();
            assetListBox.Items.Add("None");
            foreach (var entry in Asset.Entries)
            {
                assetListBox.Items.Add(entry.name);
            }
            assetListBox.SelectedIndex = 0;
        }

        private void LoadInvokeTemplate(string key)
        {
            if (_paramMap.ContainsKey(key))
            {
                var node = _paramMap[key];

                var json = JSONWriter.WriteToString(node);
                contractInputField.Text = json;

                lastParams = key;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = paramsList.Text;
            LoadInvokeTemplate(key);
        }

        private bool InitInvoke()
        {
            var json = contractInputField.Text;

            if (string.IsNullOrEmpty(json))
            {
                MessageBox.Show("Invalid input!");
                return false;
            }

            DataNode node;

            try
            {
                node = JSONReader.ReadFromString(json);
            }
            catch
            {
                MessageBox.Show("Error parsing input!");
                return false;
            }

            var items = node.GetNode("params");

            if (assetListBox.SelectedIndex > 0)
            {
                foreach (var entry in Asset.Entries)
                {
                    if (entry.name == assetListBox.SelectedItem.ToString())
                    {
                        BigInteger ammount;

                        BigInteger.TryParse(assetAmmount.Text, out ammount);

                        if (ammount > 0)
                        {
                            emulator.SetTransaction(entry.id, ammount);
                        }
                        else
                        {
                            MessageBox.Show(entry.name + " ammount must be greater than zero");
                            return false;
                        }

                        break;
                    }
                }
            }

            emulator.Reset(items);

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (InitInvoke())
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private static Dictionary<string, DataNode> _paramMap = null;

        private void RunForm_Shown(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;

            assetAmmount.Enabled = assetListBox.SelectedIndex > 0;

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
                    }
                    finally
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

                if (paramsList.SelectedIndex<0 && paramsList.Items.Count > 0)
                {
                    paramsList.SelectedIndex = 0;
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

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            assetAmmount.Enabled = assetListBox.SelectedIndex > 0;
        }

        private void paramsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1_Click(null, null);
        }
    }
}
