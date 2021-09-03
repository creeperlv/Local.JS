using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CLUNL.Utilities;
using Jint;
using Local.JS.Extension.BatchedMultiTask;
using Local.JS.Extension.IndexedFile;
using Local.JS.Extension.SimpleHttpServer;
using Local.JS.Preprocessor;
using Newtonsoft.Json;

namespace Local.JS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            string Path0 = (new FileInfo(typeof(Program).Assembly.Location)).Directory.FullName;
            Environment.CurrentDirectory = Path0;
            bool SkipPreprocess = false;
            bool ShowInfo = false;
            bool willExecute = true;
            bool ShowContent = false;
            string EntryPoint = null;
            StringBuilder Content = new();
            List<Assembly> assemblies = new();
            List<string> parameters = new();
            List<string> Flags = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                var item = args[i];
                if (item.ToUpper() == "-S")
                {
                    item = args[i + 1];
                    if (File.Exists(item))
                    {
                        Content.Append(File.ReadAllText(item));
                    }
                    i++;
                }
                else
                if (item.ToUpper() == "-L")
                {
                    item = args[i + 1];
                    if (File.Exists(item))
                    {
                        assemblies.Add(Assembly.LoadFrom((new FileInfo(item)).FullName));
                    }
                    i++;
                }
                else
                if (item.ToUpper() == "-DEFINE" || item.ToUpper() == "-DEF")
                {
                    item = args[i + 1];
                    Flags.Add(item);
                    i++;
                }
                else
                if (item.ToUpper() == "-E")
                {
                    item = args[i + 1];
                    EntryPoint = item;
                    i++;
                }
                else if (item.ToUpper() == "--VERSION")
                {
                    Console.WriteLine(">>>>>>Version Info>>>>>>");
                    {

                        Console.Write("Local.JS:");
                        Console.Write("\t\t\t\t");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(typeof(LocalJSCore).Assembly.GetName().Version);
                        Console.ResetColor();
                    }
                    {

                        Console.Write("Local.JS.Preprocessor:");
                        Console.Write("\t\t\t");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(typeof(PreProcessorCore).Assembly.GetName().Version);
                        Console.ResetColor();
                    }
                    {

                        Console.Write("Local.JS.Extension.HttpServer:");
                        Console.Write("\t\t");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(typeof(ServerCore).Assembly.GetName().Version);
                        Console.ResetColor();
                    }
                    {

                        Console.Write("Local.JS.Extension.IndexedFile:");
                        Console.Write("\t\t");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(typeof(Extension.IndexedFile.Index).Assembly.GetName().Version);
                        Console.ResetColor();
                    }
                    {

                        Console.Write("Local.JS.Extension.HttpServer:");
                        Console.Write("\t\t");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(typeof(ServerCore).Assembly.GetName().Version);
                        Console.ResetColor();
                    }
                    {

                        Console.Write("Local.JS.Extension.BatchedMultiTask:");
                        Console.Write("\t");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(typeof(TaskGroup).Assembly.GetName().Version);
                        Console.ResetColor();
                    }
                    {
                        Console.Write("Jint:");
                        Console.Write("\t\t\t\t\t");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(typeof(Engine).Assembly.GetName().Version);
                        Console.ResetColor();
                    }
                    Console.WriteLine("<<<<<<Info End<<<<<<");
                    return;
                }
                else if (item.ToUpper() == "--VIEW-INFO")
                {
                    ShowInfo = true;
                }
                else if (item.ToUpper() == "--NOEXECUTE")
                {
                    willExecute = false;
                }else if (item.ToUpper() == "--SHOW-FINAL-SCRIPT")
                {
                    ShowContent = true;
                }
                else if (item.ToUpper() == "--NOPREPROCESS")
                {
                    SkipPreprocess = true;
                }
                else if (item.ToUpper() == "--FORCEPREPROCESS")
                {
                    SkipPreprocess = false;
                }
                else
                {
                    if (File.Exists(item))
                    {
                        Environment.CurrentDirectory = new FileInfo(item).Directory.FullName;
                        if (item.ToUpper().EndsWith("JS"))
                        {
                            Content.Append(File.ReadAllText(item));
                        }
                        else if (item.ToUpper().EndsWith(".DLL"))
                        {
                            assemblies.Add(Assembly.LoadFrom((new FileInfo(item)).FullName));
                        }
                        else
                            parameters.Add(item);
                    }
                    else
                        parameters.Add(item);
                }
            }
            string RealContent = Content.ToString();
            JSInfo info = null;
            if (SkipPreprocess == false)
            {
                var di = new List<DirectoryInfo>();

                di.Add((new FileInfo(typeof(Program).Assembly.Location)).Directory);
                PreProcessorCore preProcessorCore = new PreProcessorCore(RealContent, new DirectoryInfo(Environment.CurrentDirectory), di);
                ProcessSettings settings = new ProcessSettings();
                settings.DisposeSingleLineComment = true;
                settings.PreserveSingleLineCommentInMainFile = false;
                preProcessorCore.SetProcessSettings(settings);
#if DEBUG
                Flags.Add("DEBUG");
#endif
#if RELEASE
                Flags.Add("RELEASE");
#endif
                RealContent = preProcessorCore.Process(Flags);
                info = preProcessorCore.GetInfo();
                VersionTool.SetInfo(info);
                if (ShowInfo == true)
                {
                    Console.WriteLine(">>>>>>Module Info>>>>>>");
                    Console.WriteLine("Name:" + info.Name);
                    Console.WriteLine("Author:" + info.Author);
                    Console.WriteLine("Version:" + info.Version);
                    Console.WriteLine("<<<<<<Info End<<<<<<");
                }
                List<FileInfo> fis = new List<FileInfo>();
                foreach (var item in info.UsingDLLs)
                {
                    FileInfo fileInfo = new FileInfo(item);
                    if (fileInfo.Exists)
                    {
                        bool Existed = false;
                        foreach (var f in fis)
                        {
                            if (f.FullName == fileInfo.FullName)
                            {
                                Existed = true;
                                break;
                            }
                        }
                        if (Existed == false)
                        {
                            fis.Add(fileInfo);
                        }
                    }
                    else
                    {
                        fileInfo = new FileInfo(Path.Combine(Path0, item));
                        if (fileInfo.Exists)
                        {
                            bool Existed = false;
                            foreach (var f in fis)
                            {
                                if (f.FullName == fileInfo.FullName)
                                {
                                    Existed = true;
                                    break;
                                }
                            }
                            if (Existed == false)
                            {
                                fis.Add(fileInfo);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Cannot load:" + item);
                        }
                    }
                }
                foreach (var submodule in info.SubModules)
                {

                    foreach (var item in submodule.UsingDLLs)
                    {
                        FileInfo fileInfo = new FileInfo(item);
                        if (fileInfo.Exists)
                        {
                            bool Existed = false;
                            foreach (var f in fis)
                            {
                                if (f.FullName == fileInfo.FullName)
                                {
                                    Existed = true;
                                    break;
                                }
                            }
                            if (Existed == false)
                            {
                                fis.Add(fileInfo);
                            }
                        }
                        else
                        {
                            fileInfo = new FileInfo(Path.Combine(Path0, item));
                            if (fileInfo.Exists)
                            {
                                bool Existed = false;
                                foreach (var f in fis)
                                {
                                    if (f.FullName == fileInfo.FullName)
                                    {
                                        Existed = true;
                                        break;
                                    }
                                }
                                if (Existed == false)
                                {
                                    fis.Add(fileInfo);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Cannot load:" + item);
                            }
                        }
                    }
                }
                foreach (var item in fis)
                {
                    if (item.Exists)
                    {
                        assemblies.Add(Assembly.LoadFrom(item.FullName));
                    }
                }
            }
            else
            {

                if (ShowInfo == true)
                {
                    Console.WriteLine(">>>>Warning>>>>");
                    Console.WriteLine("No Info to View");
                    Console.WriteLine("<<Warning End<<");
                }
            }
            if (ShowContent == true)
            {
                Console.WriteLine(RealContent);
            }
            if (willExecute == false)
            {
                return;
            }
            LocalJSCore localJSCore = new LocalJSCore(assemblies.ToArray());
            localJSCore.AppendProgramSegment(RealContent);
            localJSCore.ExposeType("Console", typeof(Console));
            //localJSCore.ExposeType("ServerCore", typeof(ServerCore));
            //localJSCore.ExposeType("Index", typeof(Extension.IndexedFile.Index));
            localJSCore.ExposeMethod("alert", new Action<object>(Console.WriteLine));
            localJSCore.ExposeMethod("error", new Action<object>((o) => { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(o); Console.ResetColor(); }));
            localJSCore.ExposeMethod("warn", new Action<object>((o) => { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(o); Console.ResetColor(); }));
            localJSCore.ExposeMethod("pass", new Action<object>((o) => { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(o); Console.ResetColor(); }));
            localJSCore.ExposeType("CoreVersions", typeof(CoreVersions));
            //localJSCore.ExposeType("JsonConvert", typeof(JsonConvert));
            //localJSCore.ExposeType("File", typeof(File));
            //localJSCore.ExposeType("Directory", typeof(Directory));
            //localJSCore.ExposeType("CommandLineTool", typeof(CommandLineTool));
            //localJSCore.ExposeType("TaskGroup", typeof(TaskGroup));
            localJSCore.ExposeType("VersionTool", typeof(VersionTool));
            localJSCore.ExposeType("Vector2", typeof(Vector2));
            localJSCore.ExposeType("BigInteger", typeof(BigInteger));
            localJSCore.ExposeType("BitConverter", typeof(BitConverter));
            localJSCore.ExposeObject("Core", localJSCore);
            if (info is not null)
            {
                foreach (var item in info.ExposedTypes)
                {
                    Type t = null;
                    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        t = asm.GetType(item.Value);
                        if (t is not null) break;
                    }
                    if (t is not null)
                    {
                        localJSCore.ExposeType(item.Key, t);
                    }
                    else Console.WriteLine("Cannot Expose:" + item.Value);
                }
                foreach (var submodule in info.SubModules)
                {

                    foreach (var item in submodule.ExposedTypes)
                    {
                        Type t = null;
                        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            t = asm.GetType(item.Value);
                            if (t is not null) break;
                        }
                        if (t is not null)
                        {
                            localJSCore.ExposeType(item.Key, t);
                        }
                        else Console.WriteLine("Cannot Expose:" + item.Value);
                    }
                }
            }
            if (EntryPoint is not null)
            {
                localJSCore.Execute();
                var a = localJSCore.Invoke(EntryPoint, parameters.ToArray());
                Console.WriteLine(a);
            }
            else
            {
                if (parameters.Count != 0)
                {

                    var _args = parameters.ToArray();
                    localJSCore.ExposeObject("args", _args);

                }
                else
                {
                    localJSCore.ExposeObject("args", new string[0]);

                }
                localJSCore.Execute();
            }
        }
    }
}
