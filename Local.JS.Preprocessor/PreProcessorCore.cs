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
        public List<JSInfo> SubModules = new List<JSInfo>();
        public string Name="";
        public string Author="";
        public Version Version=null;
        public List<string> Flags = new List<string>();
        public List<string> UsingDLLs = new List<string>();

    }
    public class ProcessSettings
    {
        /// <summary>
        /// Override settings about removing macros.
        /// </summary>
        public bool RemoveAllMacros = false;
        public bool RemoveUsingJSMacro = true;
        public bool RemoveDefineMacro = true;
        public bool PreserveModuleInfoMacro = true;
    }
    public class PreProcessorCore
    {
        FileInfo MainSourceFile = null;
        string ScriptContent = null;
        ProcessSettings settings = new ProcessSettings();
        DirectoryInfo MainSourceFileDirectory;
        DirectoryInfo[] Usings;
        public PreProcessorCore(string ScriptContent, DirectoryInfo MainSourceDirectory, DirectoryInfo[] Usings)
        {
            this.ScriptContent = ScriptContent;
            MainSourceFileDirectory = MainSourceDirectory;
            if (Usings is null) this.Usings = new DirectoryInfo[0];
            else this.Usings = Usings;
        }
        public PreProcessorCore(FileInfo MainSource, DirectoryInfo[] Usings)
        {
            MainSourceFile = MainSource;
            MainSourceFileDirectory = MainSource.Directory;
            if (Usings is null) this.Usings = new DirectoryInfo[0];
            else this.Usings = Usings;
        }
        public ProcessSettings GetProcessSettings() => settings;
        public void SetProcessSettings(ProcessSettings settings)
        {
            this.settings = settings;
        }
        JSInfo info = null;
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
            bool isIgnore = false;
            JSInfo info;
            if (isMainFile) info = this.info; else info = new JSInfo();
            for (int i = 0; i < lines.Length; i++)
            {
                var item = lines[i];
                if (item.Trim().StartsWith("///"))
                {
                    //Macro
                    var macro = item.Trim().Substring(3).Trim();
                    var m0 = CommandLineTool.Analyze(macro);
                    bool willDisposeLine = false;
                    switch (m0.RealParameter[0].EntireArgument.ToUpper())
                    {
                        case "DEFINE":
                            {
                                if (settings.RemoveDefineMacro)
                                    willDisposeLine = true;
                                var Key = m0.RealParameter[1].EntireArgument;
                                switch (Key.ToUpper())
                                {
                                    case "NAME":
                                        {
                                            if (settings.PreserveModuleInfoMacro && isMainFile) willDisposeLine = false;
                                            info.Name = m0.RealParameter[2].EntireArgument;
                                        }
                                        break;
                                    case "AUTHOR":
                                        {
                                            if (settings.PreserveModuleInfoMacro && isMainFile) willDisposeLine = false;
                                            info.Author = m0.RealParameter[2].EntireArgument;
                                        }
                                        break;
                                    case "VERSION":
                                        {
                                            if (settings.PreserveModuleInfoMacro && isMainFile) willDisposeLine = false;
                                            info.Version = new Version(m0.RealParameter[2].EntireArgument);
                                        }
                                        break;
                                    default:
                                        {
                                            info.Flags.Add(Key);
                                        }
                                        break;
                                }
                            }
                            break;
                        case "USING":
                            {
                                if (m0.RealParameter[1].EntireArgument.ToUpper() == "JS")
                                {
                                    if (settings.RemoveUsingJSMacro)
                                        willDisposeLine = true;
                                    var f = FindJS(m0.RealParameter[2]);
                                    if (f is not null)
                                    {
                                        lines[i] = Process(f, false);
                                    }
                                    //Pre-Combine JavaScript Codes.
                                }
                                else
                                if (m0.RealParameter[1].EntireArgument.ToUpper() == "DLL")
                                {
                                    info.UsingDLLs.Add(m0.RealParameter[2]);
                                }
                            }
                            break;
                        case "IFDEF":
                            {
                                foreach (var flag in info.Flags)
                                {
                                    if (m0.RealParameter[1].EntireArgument == flag)
                                    {
                                        isIgnore = true;
                                        break;
                                    }
                                }
                            }
                            break;
                        case "ENDIF":
                            {
                                isIgnore = false;
                            }
                            break;
                        case "IFNDEF":
                            {
                                isIgnore = true;
                                foreach (var flag in info.Flags)
                                {
                                    if (m0.RealParameter[1].EntireArgument == flag)
                                    {
                                        isIgnore = false;
                                        break;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    if (settings.RemoveAllMacros is true) willDisposeLine = true;
                    if (willDisposeLine)
                        continue;
                }
                if (isIgnore is not true)
                {
                    stringBuilder.Append(item);
                    stringBuilder.Append(Environment.NewLine);
                }
            }
            if (!isMainFile) this.info.SubModules.Add(info);
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
        public string Process(List<string> PreDefinedFlags)
        {
            if (MainSourceFile is not null)
            {
                info = new JSInfo();
                if (PreDefinedFlags is not null)
                    info.Flags = PreDefinedFlags;
                return Process(MainSourceFile, true);
            }
            else if (ScriptContent is not null)
            {
                info = new JSInfo();
                if (PreDefinedFlags is not null)
                    info.Flags = PreDefinedFlags;
                return Process(ScriptContent, true);
            }
            else throw new Exception("No input");

        }
        public string Process()
        {
            return Process(new List<string>());
        }
    }
}
