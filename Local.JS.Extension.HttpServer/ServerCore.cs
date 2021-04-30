using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Jint.Native;
using Local.JS.Extension.BatchedMultiTask;

namespace Local.JS.Extension.SimpleHttpServer
{
    public class ServerCore
    {
        static Engine ExecutingEngine;
        static HttpListener httpListener;
        static List<string> Handlers = new();
        static List<string> Addresses = new();
        static int MaxJobs = 0;
        static Action<Exception> a = (e) => { Console.WriteLine(e); };
        static string ExceptionHandler = null;
        static string _ServerName = "Local.JS.Extension.HttpServer";
        public static string ServerName { get => _ServerName; }
        public static void SetExceptionHandler(string ExceptionHandler)
        {
            ServerCore.ExceptionHandler = ExceptionHandler;
        }
        public static void SetMaxJobs(int Count)
        {
            MaxJobs = Count;
        }
        public static void SetExecutingEngine(Engine engine)
        {
            ExecutingEngine = engine;
        }
        public static void AddHandler(string FuncName)
        {
            Handlers.Add(FuncName);
        }
        public static void RemoveHandler(string FuncName)
        {
            if (Handlers.Contains(FuncName)) Handlers.Remove(FuncName);
        }
        public static void ClearHandlers()
        {
            Handlers.Clear();
        }
        public static void SetListeningAddress(string Address)
        {
            Addresses.Add(Address);
        }
        public static void RemoveListeningAddress(string Address)
        {
            Addresses.Remove(Address);
        }
        public static void ApplySettings()
        {
            _ServerName += "/" + typeof(ServerCore).Assembly.GetName().Version;
            httpListener = new HttpListener();
            tg = TaskGroup.CreateTaskGroup(MaxJobs, (a) => { if (ExceptionHandler is not null) { ExecutingEngine.Invoke(ExceptionHandler, a); } else { Console.WriteLine(a); } });
            foreach (var item in Addresses)
            {
                httpListener.Prefixes.Add(item);
            }
        }
        static bool willStop = false;
        public static void Start()
        {
            httpListener.Start();
            Task.Run(() =>
            {
                while (willStop is false)
                {
                    var c = httpListener.GetContext();
                    tg.Batch(new Action(() =>
                    {
                        if (Handlers.Count != 0)
                        {

                            foreach (var item in Handlers)
                            {
                                ExecutingEngine.Invoke(item, c);
                            }
                        }
                        else
                        {
                            SendMessage(c, "<html><body><p>Local.JS</p></body></html>", "text/html");
                        }
                    }));
                }
            });
        }
        public static void SetBufferSize(int Size)
        {
            BUF_SIZE = Size;
        }
        static int BUF_SIZE = 4096;
        public static void SendMessage(HttpListenerContext context, string Message, string MimeType = null)
        {
            context.Response.Headers.Remove(HttpResponseHeader.Server);
            context.Response.Headers.Set(HttpResponseHeader.Server, _ServerName);
            if (MimeType is not null) context.Response.ContentType = MimeType;
            else if (Message.IndexOf("<html>") != -1)
            {
                context.Response.ContentType = "text/html";
            }
            else
                context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.OutputStream.Write(Encoding.UTF8.GetBytes(Message));
            context.Response.OutputStream.Flush();
            context.Response.Close();
        }
        public static void SendFile(HttpListenerContext context, FileInfo file, string MimeType = null)
        {
            context.Response.Headers.Remove(HttpResponseHeader.Server);
            context.Response.Headers.Set(HttpResponseHeader.Server, _ServerName);
            using (var fs = file.OpenRead())
            {
                if (MimeType is not null) context.Response.ContentType = MimeType;
                byte[] b = new byte[BUF_SIZE];
                while (fs.Read(b, 0, BUF_SIZE) != -1)
                {
                    context.Response.OutputStream.Write(b);
                    context.Response.OutputStream.Flush();
                }
                context.Response.OutputStream.Flush();
                context.Response.Close();
            }
        }
        public static void SendFile(HttpListenerContext context, IndexedFile.Index index, string MimeType = null)
        {
            context.Response.Headers.Remove(HttpResponseHeader.Server);
            context.Response.Headers.Set(HttpResponseHeader.Server, _ServerName);
            using (var fs = index.CoreFile.OpenRead())
            {
                if (MimeType is not null) context.Response.ContentType = MimeType;
                else
                {
                    var Presudo = index.PseudoLocation.ToUpper();
                    if (Presudo.EndsWith(".HTM") || Presudo.EndsWith(".HTML"))
                    {
                        context.Response.ContentType = "text/html";
                    }
                }
                byte[] b = new byte[BUF_SIZE];
                while (fs.Read(b, 0, BUF_SIZE) != 0)
                {
                    context.Response.OutputStream.Write(b);
                    context.Response.OutputStream.Flush();
                }
                context.Response.OutputStream.Flush();
                context.Response.Close();
            }
        }
        static TaskGroup tg;
        public static void Stop()
        {
            tg.Dispose();
            httpListener.Close();
        }
    }
}
