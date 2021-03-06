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
        public string Name = "";
        public string Author = "";
        public Version Version = null;
        public List<string> Flags = new List<string>();
        public List<string> UsingDLLs = new List<string>();
        public Dictionary<string, string> ExposedTypes = new Dictionary<string, string>();

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
        public bool PreserveExposeTypeMacro = true;
        public bool DisposeSingleLineComment = true;
        public bool PreserveSingleLineCommentInMainFile = true;
    }
    public class PreProcessorCore
    {
        FileInfo MainSourceFile = null;
        string ScriptContent = null;
        ProcessSettings settings = new ProcessSettings();
        DirectoryInfo MainSourceFileDirectory;
        List<DirectoryInfo> Usings;
        public PreProcessorCore(string ScriptContent, DirectoryInfo MainSourceDirectory, List<DirectoryInfo> Usings)
        {
            this.ScriptContent = ScriptContent;
            MainSourceFileDirectory = MainSourceDirectory;
            if (Usings is null) this.Usings = new();
            else this.Usings = Usings;

            string Path0 = (new FileInfo(typeof(PreProcessorCore).Assembly.Location)).Directory.FullName;
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(Path0, "Include"));
                if (!di.Exists) di.Create();
                this.Usings.Add(di);
            }

        }
        public PreProcessorCore(FileInfo MainSource, List<DirectoryInfo> Usings)
        {
            MainSourceFile = MainSource;
            MainSourceFileDirectory = MainSource.Directory;
            if (Usings is null) this.Usings = new();
            else this.Usings = Usings;
            string Path0 = (new FileInfo(typeof(PreProcessorCore).Assembly.Location)).Directory.FullName;
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(Path0, "Include"));
                if (!di.Exists) di.Create();
                this.Usings.Add(di);
            }
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
            if (isMainFile) info = this.info;
            else info = new JSInfo();
            for (int i = 0; i < lines.Length; i++)
            {
                var item = lines[i];
                if (item.Trim().StartsWith("///") || item.Trim().StartsWith("#"))
                {
                    //Macro

                    var macro = item.Trim();
                    if (item.Trim().StartsWith("///"))
                    {
                        macro = macro.Substring(3).Trim();
                    }
                    else
                        macro = macro.Substring(1).Trim();
                    if (macro == "") continue;
                    var m0 = CommandLineTool.Analyze(macro);
                    bool willDisposeLine = false;
                    switch (m0.RealParameter[0].EntireArgument.ToUpper())
                    {
                        case "DEFINE":
                            {
                                if (isIgnore is not true)
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
                                                this.info.Flags.Add(Key);
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                        case "USING":
                            {
                                if (isIgnore is not true)
                                {
                                    if (m0.RealParameter[1].EntireArgument.ToUpper() == "JS")
                                    {
                                        if (settings.RemoveUsingJSMacro)
                                            willDisposeLine = true;
                                        var f = FindJS(m0.RealParameter[2].EntireArgument);
                                        if (f is not null)
                                        {
                                            item = Process(f, false);
                                            stringBuilder.Append(item);
                                            continue;// Force skip this to avoid duplicate code.
                                        }
                                        else
                                        {
                                            throw new Exception("Target JS file not found:" + m0.RealParameter[2].EntireArgument);
                                        }
                                        //Pre-Combine JavaScript Codes.
                                    }
                                    else
                                if (m0.RealParameter[1].EntireArgument.ToUpper() == "DLL")
                                    {
                                        info.UsingDLLs.Add(m0.RealParameter[2]);
                                    }
                                }
                            }
                            break;
                        case "INCLUDE":
                            {

                                if (isIgnore is not true)
                                {
                                    var f = FindJS(m0.RealParameter[1].EntireArgument);
                                    if (f is not null)
                                    {
                                        item = Process(f, false);
                                        stringBuilder.Append(item);
                                        continue;// Force skip this to avoid duplicate code.
                                    }
                                    else
                                    {
                                        throw new Exception("Target JS file not found:" + m0.RealParameter[2].EntireArgument);
                                    }
                                }
                            }
                            break;
                        case "IFDEF":
                            {
                                isIgnore = true;
                                foreach (var flag in this.info.Flags)
                                {
                                    if (m0.RealParameter[1].EntireArgument == flag)
                                    {
                                        isIgnore = false;
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
                                isIgnore = false;
                                foreach (var flag in this.info.Flags)
                                {
                                    if (m0.RealParameter[1].EntireArgument == flag)
                                    {
                                        isIgnore = true;
                                        break;
                                    }
                                }
                            }
                            break;
                        case "EXPOSETYPE":
                            {
                                if (isIgnore is not true)
                                {
                                    var Name = m0.RealParameter[1].EntireArgument;
                                    var Path = m0.RealParameter[2].EntireArgument;
                                    if (settings.PreserveExposeTypeMacro == true) willDisposeLine = false;
                                    if (!info.ExposedTypes.ContainsKey(Name)) info.ExposedTypes.Add(Name, Path);
                                    else info.ExposedTypes[Name] = Path;
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
                else if (item.Trim().StartsWith("//"))
                {
                    //isIgnore = settings.DisposeSingleLineComment && (!settings.PreserveSingleLineCommentInMainFile || isMainFile ==false);
                    continue;
                }
                if (isIgnore is not true)
                {
                    if (item.Trim().StartsWith("#"))
                    {
                        stringBuilder.Append("///");
                        stringBuilder.Append(item.Trim().Substring(1));
                    }
                    else
                    {
                        stringBuilder.Append(item);

                    }
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
                    this.info.Flags = PreDefinedFlags;
                return Process(MainSourceFile, true);
            }
            else if (ScriptContent is not null)
            {
                info = new JSInfo();
                if (PreDefinedFlags is not null)
                    this.info.Flags = PreDefinedFlags;
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
