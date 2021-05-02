using System;
using System.Collections.Generic;
using System.IO;

namespace Local.JS.Preprocessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FileInfo MainSource = null;
            List<DirectoryInfo> di = new List<DirectoryInfo>();
            List<string> Flags = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                var item = args[i];
                if (item.ToUpper() == "-USING")
                {
                    di.Add(new DirectoryInfo(args[i + 1]));
                    i++;

                }
                else if (item.ToUpper() == "-DEFINE" || item.ToUpper() == "-DEF")
                {
                    Flags.Add(args[i + 1]);
                    i++;

                }
                if (File.Exists(item))
                {
                    MainSource = new FileInfo(item);
                }
            }
            PreProcessorCore core = new PreProcessorCore(MainSource, di.ToArray());
        }
    }
}
