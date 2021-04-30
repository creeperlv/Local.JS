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
    }
    public class PreProcessorCore
    {
        FileInfo MainSourceFile;
        DirectoryInfo[] Usings;
        public PreProcessorCore(FileInfo MainSource, DirectoryInfo[] Usings)
        {
            MainSourceFile = MainSource;
            if (Usings is not null) this.Usings = new DirectoryInfo[0];
            else this.Usings = Usings;
        }
        public FileInfo FindJS(string Name)
        {
            var localF = (Path.Combine(MainSourceFile.Directory.FullName, Name));
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
        internal string Process(FileInfo file, bool isMainFile)
        {
            var lines = File.ReadAllLines(file.FullName);
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
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return null;
        }
        public string Process()
        {
            return Process(MainSourceFile, true);
        }
    }
}
