using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLUNL.Utilities;

namespace Local.JS.Preprocessor
{
    public class JSInfo
    {
        public string Name;
        public string Author;
        public Version Version;
        public List<string> UsingDLLs;

    }
    public class PreProcessorCore
    {
        FileInfo MainSourceFile = null;
        string ScriptContent = null;
        DirectoryInfo MainSourceFileDirectory;
        DirectoryInfo[] Usings;
        public PreProcessorCore(string ScriptContent, DirectoryInfo MainSourceDirectory, DirectoryInfo[] Usings)
        {
            this.ScriptContent = ScriptContent;
            MainSourceFileDirectory = MainSourceDirectory;
            if (Usings is not null) this.Usings = new DirectoryInfo[0];
            else this.Usings = Usings;
        }
        public PreProcessorCore(FileInfo MainSource, DirectoryInfo[] Usings)
        {
            MainSourceFile = MainSource;
            MainSourceFileDirectory = MainSource.Directory;
            if (Usings is not null) this.Usings = new DirectoryInfo[0];
            else this.Usings = Usings;
        }
        JSInfo info=null;
        public JSInfo GetInfo() => info;
        public FileInfo FindJS(string Name)
        {
            var localF = (Path.Combine(MainSourceFileDirectory.FullName, Name));
            if (File.Exists(localF))
            {
                return new FileInfo(localF);
            }
            else
            {
                foreach (var item in Usings)
                {
                    var GlobalF = (Path.Combine(item.FullName, Name));
                    if (File.Exists(GlobalF))
                    {
                        return new FileInfo(GlobalF);
                    }
                }
            }
            return null;
        }
        internal string RealProcess(string[] lines, bool isMainFile)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                var item = lines[i];
                if (item.Trim().StartsWith("///"))
                {
                    //Macro
                    var m0 = CommandLineTool.Analyze(item);
                    switch (m0.RealParameter[0].EntireArgument.ToUpper())
                    {
                        case "DEFINE":
                            break;
                        case "USING":
                            {
                                if (m0.RealParameter[1].EntireArgument.ToUpper() == "JS")
                                {
                                    var f = FindJS(m0.RealParameter[2]);
                                    if (f is not null)
                                    {
                                        lines[i] = Process(f, false);
                                    }
                                    //Pre-Combine JavaScript Codes.
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                stringBuilder.Append(item);
            }
            return stringBuilder.ToString();
        }
        internal string Process(string content, bool isMainFile)
        {

            var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.TrimEntries);

            return RealProcess(lines, isMainFile);
        }
        internal string Process(FileInfo file, bool isMainFile)
        {
            var lines = File.ReadAllLines(file.FullName);
            return RealProcess(lines, isMainFile);
        }
        public string Process()
        {
            if (MainSourceFile is not null)
            {
                info = new JSInfo();
                return Process(MainSourceFile, true);
            }
            else if (ScriptContent is not null)
            {
                info = new JSInfo();
                return Process(ScriptContent, true);
            }
            else throw new Exception("No input");
        }
    }
}
