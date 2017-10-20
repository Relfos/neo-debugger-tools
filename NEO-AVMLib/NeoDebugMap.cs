using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.Tools.AVM
{
    public class DebugMapEntry
    {
        public string url;
        public int line;

        public int startOfs;
        public int endOfs;
    }

    public class NeoMapFile
    {
        private List<DebugMapEntry> _entries = new List<DebugMapEntry>();
        public IEnumerable<DebugMapEntry> Entries { get { return _entries; } }

        public void LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            var lines = File.ReadAllLines(path);
            _entries = new List<DebugMapEntry>();
            foreach (var line in lines)
            {
                var temp = line.Split(new char[] { ',' }, 4);

                var entry = new DebugMapEntry();
                int.TryParse(temp[0], out entry.startOfs);
                int.TryParse(temp[1], out entry.endOfs);
                int.TryParse(temp[2], out entry.line);
                entry.url = temp[3];
                _entries.Add(entry);
            }
        }

        /// <summary>
        /// Calculates the source code line that maps to the script offset.
        /// </summary>
        public int ResolveLine(int ofs)
        {
            foreach (var entry in this.Entries)
            {
                if (ofs >= entry.startOfs && ofs <= entry.endOfs)
                {
                    return entry.line;
                }
            }

            throw new Exception("Offset cannot be mapped");
        }
    }

}
