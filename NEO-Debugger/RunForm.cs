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
        public ABI abi;

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

        private Dictionary<string, string> _lastParams = new Dictionary<string, string>();

        private void LoadFunction(string key)
        {
            if (abi.functions.ContainsKey(key))
            {
                var f = abi.functions[key];

                inputGrid.Rows.Clear();

                if (f.inputs != null)
                {
                    foreach (var p in f.inputs)
                    {
                        var temp = (key + "_" + f.name).ToLower();
                        string val = "";

                        if (_lastParams.ContainsKey(temp))
                        {
                            val = _lastParams[temp];
                        }

                        inputGrid.Rows.Add(new object[] { p.name, val});
                    }
                }

                lastParams = key;

                button1.Enabled = true;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = paramsList.Text;
            LoadFunction(key);
        }

        private bool InitInvoke()
        {

            var key = paramsList.Text;
            var f = abi.functions[key];

            var argList = "";

            if (f.inputs != null)
            {
                int index = 0;
                foreach (var p in f.inputs)
                {
                    var temp = ($"{key}_{f.name}").ToLower();
                    var val = inputGrid.Rows[index].Cells[1].Value;

                    if (index>0)
                    {
                        argList += ",";
                    }

                    switch (p.type.ToLower())
                    {
                        case "string": val = $"\"{val}\""; break;
                    }

                    argList += val;
                    index++;
                }
            }

            if (key != abi.entryPoint.name)
            {
                if (f.inputs == null || f.inputs.Length == 0)
                {
                    argList = "[null]";
                }
                argList = $"\"{key.ToLowerInvariant()}\", {argList}";
            }

            string json = "{\"params\": ["+ argList + "]}";

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

        private void RunForm_Shown(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;

            inputGrid.AllowUserToAddRows = false;

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

            paramsList.Items.Clear();

            foreach (var f in abi.functions.Values)
            {
                paramsList.Items.Add(f.name);
            }

            int mainItem = paramsList.FindString("Main");
            if (mainItem >= 0) paramsList.SetSelected(mainItem, true);

            /*if (_paramMap != null)
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

            }*/
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
