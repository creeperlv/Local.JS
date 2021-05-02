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
            string Output=null;
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
            if(Output is null)
            {
                Output = Path.Combine(MainSource.Directory.FullName, "_" + MainSource.Name);
            }

            PreProcessorCore core = new PreProcessorCore(MainSource, di.ToArray());
            var content=core.Process(Flags);
            try
            {
                File.Delete(Output);
            }
            catch (Exception)
            {
            }
            File.WriteAllText(content, Output);
        }
    }
}
