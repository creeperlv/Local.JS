using System;
using System.IO;

namespace Local.JS.Preprocessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FileInfo MainSource;
            for (int i = 0; i < args.Length; i++)
            {
                var item = args[i];
                if (File.Exists(item))
                {
                    MainSource = new FileInfo(item);
                }
            }
        }
    }
}
