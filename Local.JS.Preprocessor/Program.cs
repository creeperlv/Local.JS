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
            ProcessSettings settings = new ProcessSettings();
            for (int i = 0; i < args.Length; i++)
            {
                var item = args[i];
                if (item.ToUpper() == "-USING")
                {
                    di.Add(new DirectoryInfo(args[i + 1]));
                    i++;

                }
                else if (item.ToUpper() == "--REMOVE-ALL-MACROS")
                {
                    settings.RemoveAllMacros = true;
                }
                else if (item.ToUpper() == "--NOT-REMOVE-ALL-MACROS")
                {
                    settings.RemoveAllMacros = false;
                }
                else if (item.ToUpper() == "--REMOVE-USING-JS-MACRO")
                {
                    settings.RemoveUsingJSMacro = true;
                }
                else if (item.ToUpper() == "--NOT-REMOVE-USING-JS-MACRO")
                {
                    settings.RemoveUsingJSMacro = false;
                }
                else if (item.ToUpper() == "--REMOVE-DEFINE-MACRO")
                {
                    settings.RemoveDefineMacro = true;
                }
                else if (item.ToUpper() == "--NOT-REMOVE-DEFINE-MACRO")
                {
                    settings.RemoveDefineMacro = false;
                }
                else if (item.ToUpper() == "--PRESERVE-MODULE-INFO-MACRO")
                {
                    settings.PreserveModuleInfoMacro = true;
                }
                else if (item.ToUpper() == "--NOT-PRESERVE-MODULE-INFO-MACRO")
                {
                    settings.PreserveModuleInfoMacro = false;
                }
                else if (item.ToUpper() == "--PRESERVE-SINGLE-LINE-COMMENTS")
                {
                    settings.DisposeSingleLineComment = false;
                }
                else if (item.ToUpper() == "--NOT-PRESERVE-SINGLE-LINE-COMMENTS")
                {
                    settings.DisposeSingleLineComment = true;
                }else if (item.ToUpper() == "--PRESERVE-SINGLE-LINE-COMMENTS-IN-MAIN-FILE")
                {
                    settings.PreserveSingleLineCommentInMainFile = true;
                }else if (item.ToUpper() == "--NOT-PRESERVE-SINGLE-LINE-COMMENTS-IN-MAIN-FILE")
                {
                    settings.PreserveSingleLineCommentInMainFile = false;
                }
                else if (item.ToUpper() == "-DEFINE" || item.ToUpper() == "-DEF")
                {
                    Flags.Add(args[i + 1]);
                    i++;

                }else if (item.ToUpper() == "-O")
                {
                    Output = args[i + 1];
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

            PreProcessorCore core = new PreProcessorCore(MainSource, di);
            core.SetProcessSettings(settings);
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
