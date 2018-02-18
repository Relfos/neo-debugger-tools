using LunarParser;
using LunarParser.JSON;
using Neo.Cryptography;
using Neo.Debugger.Utils;
using Neo.Emulator;
using Neo.Emulator.API;
using Neo.Emulator.Utils;
using NeoLux;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace Neo.Debugger.Forms
{
    public partial class RunForm : Form
    {
        public MainForm mainForm;
        public NeoEmulator emulator;
        public ABI abi;

        private string lastParams = null;

        public RunForm()
        {
            InitializeComponent();

            assetComboBox.Items.Clear();
            assetComboBox.Items.Add("None");
            foreach (var entry in Asset.Entries)
            {
                assetComboBox.Items.Add(entry.name);
            }
            assetComboBox.SelectedIndex = 0;

            triggerComboBox.SelectedIndex = 0;
            witnessComboBox.SelectedIndex = 0;
        }

        private Dictionary<string, object> _lastParams = new Dictionary<string, object>();

        private AVMFunction currentMethod;

        private void LoadFunction(string key)
        {
            if (abi.functions.ContainsKey(key))
            {
                currentMethod = abi.functions[key];

                inputGrid.Rows.Clear();

                if (currentMethod.inputs != null)
                {
                    foreach (var p in currentMethod.inputs)
                    {
                        var param_key = (currentMethod.name + "_" + p.name).ToLower();
                        object val = "";

                        if (_lastParams.ContainsKey(param_key))
                        {
                            val = _lastParams[param_key];
                        }

                        inputGrid.Rows.Add(new object[] { p.name, val });

                        int rowIndex = inputGrid.Rows.Count - 1;

                        if (!_lastParams.ContainsKey(param_key))
                        {
                            EnablePlaceholderText(rowIndex, 1, p);
                        }
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

        private void ResetTabs()
        {
            this.runTabs.SelectedTab = methodTab;
        }

        private bool IsHex(string chars)
        {
            if (string.IsNullOrEmpty(chars)) return false;
            if (chars.Length % 2 != 0) return false;

            bool isHex;
            foreach (var c in chars)
            {
                isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!isHex)
                    return false;
            }
            return true;
        }

        private void ShowArgumentError(AVMFunction f, int index, object val)
        {
            string error;

            if (val == null || string.IsNullOrEmpty(val.ToString()))
            {
                error = "Missing";
            }
            else
            {
                error = "Invalid format in ";
            }

            MessageBox.Show($"{error} argument #{index+1} (\"{f.inputs[index].name}\") of {f.name} method");
            ResetTabs();
        }

        private bool InitInvoke()
        {

            var key = paramsList.Text;
            var f = abi.functions[key];

            var ws = witnessComboBox.SelectedItem.ToString().Replace(" ", "");
            if (!Enum.TryParse<CheckWitnessMode>(ws, out emulator.checkWitnessMode))
            {
                return false;
            }

            var ts = triggerComboBox.SelectedItem.ToString().Replace(" ", "");
            if (!Enum.TryParse<TriggerType>(ts, out emulator.currentTrigger))
            {
                return false;
            }

            var argList = "";

            if (f.inputs != null)
            {
                int index = 0;
                foreach (var p in f.inputs)
                {
                    var temp = ($"{key}_{f.name}").ToLower();
                    var name = inputGrid.Rows[index].Cells[0].Value;

                    object val;

                    // detect placeholder
                    if (inputGrid.Rows[index].Cells[1].Style.ForeColor == Color.Gray)
                    {
                        val = "";
                    }
                    else
                    {
                        val = ReadCellVal(index, 1);
                    }

                    if (val == null)
                    {
                        val = ""; // temporary hack, necessary to avoid VM crash
                    }

                    if (val != null && !val.Equals(""))
                    {
                        var param_key = (f.name + "_" + p.name).ToLower();
                        _lastParams[param_key] = val;
                    }

                    if (index > 0)
                    {
                        argList += ",";
                    }

                    if (p.type.Contains("Array"))
                    {
                        var s = val.ToString();

                        if (s.StartsWith("[") && s.EndsWith("]"))
                        {
                            val = s;
                        }
                        else
                        if (IsHex(s))
                        {
                            var bytes = s.HexToBytes();
                            s = "";
                            foreach (var b in bytes)
                            {
                                if (s.Length > 0) s += ",";
                                s += b.ToString();
                            }
                            s = $"[{s}]";
                        }
                        else
                        {
                            ShowArgumentError(f, index, val);
                            return false;
                        }
                    }
                    else
                        switch (p.type.ToLower())
                        {
                            case "string":
                                {
                                    var s = val.ToString();
                                    if (!s.StartsWith("\"") || !s.EndsWith("\""))
                                    {
                                        ShowArgumentError(f, index, val);
                                        return false;
                                    }

                                    break;
                                }

                            case "integer":
                                {
                                    BigInteger n;
                                    var s = val.ToString();
                                    if (string.IsNullOrEmpty(s) || !BigInteger.TryParse(s, out n))
                                    {
                                        ShowArgumentError(f, index, val);
                                        ResetTabs();
                                        return false;
                                    }
                                    break;
                                }

                            case "boolean":
                                {
                                    switch (val.ToString().ToLower())
                                    {
                                        case "true": val = true; break;
                                        case "false": val = false; break;
                                        default:
                                            {
                                                ShowArgumentError(f, index, val);
                                                ResetTabs();
                                                return false;
                                            }

                                    }
                                    break;
                                }
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
                var operation = Char.ToLowerInvariant(key[0]) + key.Substring(1);
                argList = $"\"{operation}\", {argList}";
            }

            string json = "{\"params\": [" + argList + "]}";

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
                ResetTabs();
                return false;
            }

            var items = node.GetNode("params");

            if (assetComboBox.SelectedIndex > 0)
            {
                foreach (var entry in Asset.Entries)
                {
                    if (entry.name == assetComboBox.SelectedItem.ToString())
                    {
                        BigInteger ammount;

                        BigInteger.TryParse(assetAmount.Text, out ammount);

                        if (ammount > 0)
                        {
                            emulator.SetTransaction(entry.id, ammount);
                        }
                        else
                        {
                            MessageBox.Show(entry.name + " amount must be greater than zero");
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

        private string currentContractName = "";

        private void ReloadContract()
        {
            if (currentContractName == abi.fileName)
            {
                return;
            }

            currentContractName = abi.fileName;

            paramsList.Items.Clear();

            foreach (var f in abi.functions.Values)
            {
                paramsList.Items.Add(f.name);
            }

            int mainItem = paramsList.FindString(abi.entryPoint.name);
            if (mainItem >= 0) paramsList.SetSelected(mainItem, true);
        }

        private void RunForm_Shown(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;

            inputGrid.AllowUserToAddRows = false;

            assetAmount.Enabled = assetComboBox.SelectedIndex > 0;

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

            privateKeyInput.Text = mainForm.settings.lastPrivateKey;

            ReloadContract();
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            assetAmount.Enabled = assetComboBox.SelectedIndex > 0;
        }

        private void paramsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1_Click(null, null);
        }

        private string ReadCellVal(int row, int col)
        {
            var temp = inputGrid.Rows[row].Cells[col].Value;
            var val = temp != null ? temp.ToString() : null;
            return val;
        }

        private void EnablePlaceholderText(int row, int col, AVMInput p)
        {
            var s = p.type;

            if (p.type.Contains("Array"))
            {
                s += " (Eg: [1, 2, \"something\"]";
            }

            var curContent = ReadCellVal(row, col);

            if (curContent != s && !string.IsNullOrEmpty(curContent))
            {
                return;
            }

            inputGrid.Rows[row].Cells[col].Style.ForeColor = Color.Gray;
            inputGrid.Rows[row].Cells[col].Value = s;
        }

        private void DisablePlaceholderText(int row, int col)
        {
            inputGrid.Rows[row].Cells[col].Style.ForeColor = Color.Black;
            inputGrid.Rows[row].Cells[col].Value = "";
        }

        private void VerifyPlaceholderAt(int row, int col)
        {
            if (col == 1 && currentMethod != null && !editMode)
            {
                var val = ReadCellVal(row, col);

                if (string.IsNullOrEmpty(val))
                {
                    var p = currentMethod.inputs[row];
                    EnablePlaceholderText(row, col, p);
                }
                else
                {
                    inputGrid.Rows[row].Cells[col].Style.ForeColor = Color.Black;
                }
            }

        }

        private void inputGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (!editMode)
            {
                VerifyPlaceholderAt(e.RowIndex, e.ColumnIndex);
            }
        }

        private void inputGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && currentMethod != null)
            {
                var col = inputGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor;
                if (col == Color.Gray)
                {
                    DisablePlaceholderText(e.RowIndex, e.ColumnIndex);
                }
            }
        }

        private bool editMode = false;
        private int editRow;

        private void inputGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            editMode = true;
            editRow = e.RowIndex;
        }

        private void inputGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            editMode = false;
            VerifyPlaceholderAt(e.RowIndex, e.ColumnIndex);
        }

        private static KeyPair GetKeyFromString(string key)
        {
            if (key.Length == 52)
            {
                return KeyPair.FromWIF(key);
            }
            else
            if (key.Length == 64)
            {
                var keyBytes = key.HexToBytes();
                return new KeyPair(keyBytes);
            }
            else
            {
                return null;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            var keyPair = GetKeyFromString(privateKeyInput.Text);
            if (keyPair != null)
            {
                Runtime.invokerKeys = keyPair;
                addressLabel.Text = Runtime.invokerKeys.address;

                mainForm.settings.lastPrivateKey = privateKeyInput.Text;
                mainForm.settings.Save();
            }
            else
            {
                MessageBox.Show("Invalid private key, length should be 52 or 64");
            }
        }

        private void inputGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (editMode)
            {
                editMode = false;            
                VerifyPlaceholderAt(editRow, 1);
            }

        }
    }
}
