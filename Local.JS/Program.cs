﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CLUNL.Utilities;
using Jint;
using Local.JS.Extension.BatchedMultiTask;
using Local.JS.Extension.SimpleHttpServer;
using Newtonsoft.Json;

namespace Local.JS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            bool SkipPreprocess=true;
            string EntryPoint = null;
            StringBuilder Content = new();
            List<Assembly> assemblies = new();
            List<string> parameters = new();
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
                if (item.ToUpper() == "-E")
                {
                    item = args[i + 1];
                    EntryPoint = item;
                    i++;
                }else if (item.ToUpper() == "--NOPREPROCESS")
                {
                    SkipPreprocess = true;
                }else if (item.ToUpper() == "--FORCEPREPROCESS")
                {
                    SkipPreprocess = false;
                }
                else
                {
                    if (File.Exists(item))
                    {
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
            LocalJSCore localJSCore = new LocalJSCore(assemblies.ToArray());
            if (SkipPreprocess == false)
            {

            }
            localJSCore.AppendProgramSegment(Content.ToString());
            localJSCore.ExposeType("Console", typeof(Console));
            localJSCore.ExposeType("ServerCore", typeof(ServerCore));
            localJSCore.ExposeType("IndexedFile_Index", typeof(Extension.IndexedFile.Index));
            localJSCore.ExposeMethod("alert", new Action<object>(Console.WriteLine));
            localJSCore.ExposeMethod("error", new Action<object>((o) => { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(o); Console.ResetColor(); }));
            localJSCore.ExposeMethod("warn", new Action<object>((o) => { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(o); Console.ResetColor(); }));
            localJSCore.ExposeMethod("pass", new Action<object>((o) => { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(o); Console.ResetColor(); }));
            localJSCore.ExposeType("CoreVersions", typeof(CoreVersions));
            localJSCore.ExposeType("JsonConvert", typeof(JsonConvert));
            localJSCore.ExposeType("File", typeof(File));
            localJSCore.ExposeType("Directory", typeof(Directory));
            localJSCore.ExposeType("CommandLineTool", typeof(CommandLineTool));
            localJSCore.ExposeType("TaskGroup", typeof(TaskGroup));
            localJSCore.ExposeObject("Core", localJSCore);
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
                localJSCore.Execute();
            }
        }
    }
}
