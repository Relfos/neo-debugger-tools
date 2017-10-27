using System;

namespace Neo.Tools.AVM
{
    public static class FormattingUtils
    {
        public static string OutputLine(string col1, string col2, string col3)
        {
            int colSize = 14;
            return col1.PadRight(colSize) + col2.PadRight(colSize) + col3;
        }

        public static string OutputData(byte[] data, bool addQuotes)
        {
            for (int i = 0; i < data.Length; i++)
            {
                var c = (char)data[i];
                var isValidText = char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c);
                if (!isValidText)
                {
                    return OutputHex(data);
                }
            }

            var result = System.Text.Encoding.ASCII.GetString(data);

            if (addQuotes)
            {
                result = '"' + result + '"';
            }

            return result;
        }

        public static string OutputHex(byte[] data)
        {
            string hex = BitConverter.ToString(data);
            return hex;
        }
    }
}
