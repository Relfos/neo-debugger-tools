using LunarParser;
using LunarParser.JSON;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.Debugger
{
    public class Settings
    {
        public string lastOpenedFile;

        private string fileName;
        private string path;


        public Settings(string path)
        {
            this.path = path;
            this.fileName = path + "/settings.json";

            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);
                var root = JSONReader.ReadFromString(json);
                root = root["settings"];

                this.lastOpenedFile = root.GetString("lastfile");
            }
        }

        public void Save()
        {
            var root = DataNode.CreateObject("settings");
            root.AddField("lastfile", this.lastOpenedFile);

            var json = JSONWriter.WriteToString(root);

            Directory.CreateDirectory(this.path);

            File.WriteAllText(fileName, json);
        }
    }
}
