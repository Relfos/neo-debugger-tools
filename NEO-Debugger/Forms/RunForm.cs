using LunarParser;
using LunarParser.JSON;
using Neo.Cryptography;
using Neo.Debugger.Utils;
using Neo.Emulator;
using Neo.Emulator.API;
using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace Neo.Debugger.Forms
{
    public partial class RunForm : Form
    {
        private Settings _settings;
        private NeoEmulator _emulator;
        private ABI _abi;
        private TestSuite _testSuite;
        
        

        public RunForm(Settings settings, DebugManager debugger)
        {
            InitializeComponent();
            _settings = settings;
            _testSuite = debugger.Tests;
            _emulator = debugger.Emulator;
            _abi = debugger.ABI;

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

        private AVMFunction currentMethod;

        private void LoadFunction(string key)
        {
            if (_abi.functions.ContainsKey(key))
            {
                currentMethod = _abi.functions[key];

                inputGrid.Rows.Clear();

                if (currentMethod.inputs != null)
                {
                    foreach (var p in currentMethod.inputs)
                    {
                        var param_key = (currentContractName + "_" + currentMethod.name + "_" + p.name).ToLower();
                        object val = "";

                        bool isEmpty = true;

                        if (_settings.lastParams.ContainsKey(param_key))
                        {
                            val = _settings.lastParams[param_key];
                            isEmpty = false;
                        }

                        inputGrid.Rows.Add(new object[] { p.name, val });

                        int rowIndex = inputGrid.Rows.Count - 1;

                        if (isEmpty)
                        {
                            EnablePlaceholderText(rowIndex, 1, p);
                        }
                    }
                }

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

        public static bool IsValidWallet(string address)
        {
            if (string.IsNullOrEmpty(address) || address[0]!='A')
            {
                return false;
            }

            try
            {
                var buffer = address.Base58CheckDecode();
                return buffer != null && buffer.Length > 0;
            }
            catch
            {
                return false;
            }

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

        private string BytesToString(byte[] bytes)
        {
            var s = "";
            foreach (var b in bytes)
            {
                if (s.Length > 0) s += ",";
                s += b.ToString();
            }
            s = $"[{s}]";
            return s;
        }

        private bool InitInvoke()
        {

            var key = paramsList.Text;
            var f = _abi.functions[key];

            var ws = witnessComboBox.SelectedItem.ToString().Replace(" ", "");
            if (!Enum.TryParse<CheckWitnessMode>(ws, out _emulator.checkWitnessMode))
            {
                return false;
            }

            var ts = triggerComboBox.SelectedItem.ToString().Replace(" ", "");
            if (!Enum.TryParse<TriggerType>(ts, out _emulator.currentTrigger))
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
                        var param_key = (currentContractName + "_" + f.name + "_" + p.name).ToLower();
                        _settings.lastParams[param_key] = val.ToString();
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
                            s = BytesToString(bytes);
                        }
                        else
                        if (IsValidWallet(s))
                        {
                            var bytes = s.Base58CheckDecode();
                            var scriptHash = Crypto.Default.ToScriptHash(bytes);
                            bytes = scriptHash.ToArray();
                            s = BytesToString(bytes);
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

            if (key != _abi.entryPoint.name)
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
                            _emulator.SetTransaction(entry.id, ammount);
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

            _emulator.Reset(items);
            _settings.Save();

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (InitInvoke())
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        public string currentContractName = "";
        private string lastContractName = "";

        private void ReloadContract()
        {
            if (currentContractName == lastContractName)
            {
                return;
            }

            lastContractName = currentContractName;

            paramsList.Items.Clear();

            foreach (var f in _abi.functions.Values)
            {
                paramsList.Items.Add(f.name);
            }

            int mainItem = paramsList.FindString(_abi.entryPoint.name);
            if (mainItem >= 0) paramsList.SetSelected(mainItem, true);

            testCasesList.Items.Clear();
            foreach (var entry in _testSuite.cases.Keys)
            {
                testCasesList.Items.Add(entry);
            }
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
                    Runtime.invokerKeys = new KeyPair(privKey);
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

            privateKeyInput.Text = _settings.lastPrivateKey;

            ReloadContract();
        }
        
        private void assetComboBox_SelectedIndexChanged(object sender, EventArgs e)
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

                _settings.lastPrivateKey = privateKeyInput.Text;
                _settings.Save();
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


        private string ParseNode(DataNode node, int index)
        {
            string val;

            if (node.ChildCount > 0)
            {
                val = "";

                foreach (var child in node.Children)
                {
                    if (val.Length > 0) val += ", ";

                    val += ParseNode(child, -1);
                }
                val = $"[{val}]";
            }
            else
            if (node.Kind == NodeKind.Null)
            {
                val = "[]";
            }
            else
            if (node.Kind == NodeKind.Numeric || node.Kind == NodeKind.Boolean)
            {
                val = node.Value;
            }
            else
            if (node.Kind == NodeKind.String)
            {
                val = $"\"{node.Value}\"";

            }
            else
            {
                val = node.Value;
            }

            return val;
        }

        private void testCasesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = testCasesList.Text;
            var testCase = _testSuite.cases[key];
            var methodName = testCase.method != null ? testCase.method : _abi.entryPoint.name;

            for (int i=0; i<paramsList.Items.Count; i++)
            {
                if (paramsList.Items[i].ToString() == methodName)
                {
                    paramsList.SelectedIndex = i;

                    for (int j=0; j<inputGrid.RowCount; j++)
                    {
                        string val;

                        if (testCase.args != null && j < testCase.args.ChildCount)
                        {                            
                            var node = testCase.args[j];
                            val = ParseNode(node, j);
                        }
                        else
                        {
                            val = "";
                        }

                        inputGrid.Rows[j].Cells[1].Value = val;
                    }

                    break;
                }
            }
        }
    }
}
