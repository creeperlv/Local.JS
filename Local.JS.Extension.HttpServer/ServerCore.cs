using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        static bool EnableRange = false;
        static int MaxJobs = 0;
        static Action<Exception> a = (e) => { Console.WriteLine(e); };
        static string ExceptionHandler = null;
        static string _ServerName = "Local.JS.Extension.HttpServer";
        public static string ServerName { get => _ServerName; }
        public static void SetRangeAvailability(bool value)
        {
            EnableRange = value;
        }
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
            tg=TaskGroup.GetDefaultTaskGroup();//= TaskGroup.CreateTaskGroup(MaxJobs, (a) => { if (ExceptionHandler is not null) { ExecutingEngine.Invoke(ExceptionHandler, a); } else { Console.WriteLine(a); } });
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
            if (EnableRange == true)
                context.Response.AddHeader("Accept-Ranges", "bytes");
            else
                context.Response.AddHeader("Accept-Ranges", "none");
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
        public static void SendFile(HttpListenerContext context, FileInfo file, string MimeType = null, string PseudoPath = null)
        {

            if (EnableRange == true)
                context.Response.AddHeader("Accept-Ranges", "bytes");
            else
                context.Response.AddHeader("Accept-Ranges", "none");
            context.Response.Headers.Remove(HttpResponseHeader.Server);
            context.Response.Headers.Set(HttpResponseHeader.Server, _ServerName);
            if (PseudoPath is null) PseudoPath = "" + file.FullName;
            List<Range> ranges = new List<Range>(); string range = null;
            using (var fs = file.OpenRead())
            {
                if (EnableRange == true)
                    try
                    {
                        range = context.Request.Headers["Range"];
                        if (range != null)
                        {

                            range = range.Trim();
                            range = range.Substring(6);
                            var rs = range.Split(',');
                            foreach (var item in rs)
                            {
                                ranges.Add(Range.FromString(item));
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                if (MimeType is not null) context.Response.ContentType = MimeType;
                else
                {
                    var nameparts = PseudoPath.ToUpper().Split('.');
                    if (nameparts.Last() == ".HTM" || nameparts.Last() == ".HTML")
                    {
                        context.Response.ContentType = "text/html";
                    }
                }
                if (ranges.Count == 0)
                {

                    byte[] b = new byte[BUF_SIZE];
                    while (fs.Read(b, 0, BUF_SIZE) != -1)
                    {
                        context.Response.OutputStream.Write(b);
                        context.Response.OutputStream.Flush();
                    }
                }
                else

                {
                    context.Response.StatusCode = 206;
                    context.Response.StatusDescription = "Partial Content";
                    var OriginalContentType = "Content-Type: " + context.Response.ContentType;
                    context.Response.ContentType = "multipart/byteranges;";
                    var NewLine = Environment.NewLine;
                    foreach (var item in ranges)
                    {
                        context.Response.Headers.Add(HttpResponseHeader.ContentRange, "bytes " + item.ToString() + "/" + fs.Length);
                        long length = 0;
                        long L = 0;
                        {
                            //Calculate length to send and left-starting index.
                            if (item.R == long.MinValue || item.R > fs.Length)
                            {
                                length = fs.Length - item.L;
                            }
                            else if (item.L == long.MinValue || item.L < 0)
                            {
                                length = item.R;
                            }
                            else
                            {
                                length = item.R - item.L;
                            }
                            if (item.L != long.MinValue)
                            {
                                L = item.L;
                            }
                        }
                        fs.Seek(L, SeekOrigin.Begin);
                        int _Length;
                        byte[] buf = new byte[BUF_SIZE];
                        while (L < length)
                        {
                            if (length - L > BUF_SIZE)
                            {
                                L += (_Length = fs.Read(buf, 0, BUF_SIZE));
                            }
                            else
                            {
                                L += (_Length = fs.Read(buf, 0, (int)(length - L)));
                            }
                            context.Response.OutputStream.Write(buf, 0, _Length);
                            context.Response.OutputStream.Flush();
                        }
                        break;
                    }
                }
                context.Response.OutputStream.Flush();
                context.Response.Close();
            }
        }
        public static void SendFile(HttpListenerContext context, FileInfo file, string MimeType = null)
        {
            SendFile(context, file, MimeType, null);
        }
        public static void SendFile(HttpListenerContext context, IndexedFile.Index index, string MimeType = null)
        {
            SendFile(context, index.CoreFile, null, index.PseudoLocation);
        }
        static TaskGroup tg;
        public static void Stop()
        {
            tg.Dispose();
            httpListener.Close();
        }
    }
    public struct Range
    {
        public static Range Empty = new Range() { L = long.MinValue, R = long.MinValue };
        public long L;
        public long R;
        public override string ToString()
        {
            if (L == long.MinValue)
            {
                if (R == long.MinValue)
                    return $"-";
                return $"-{R}";
            }
            else if (R == long.MinValue)
                return $"{L}-";
            else return $"{L}-{R}";
        }
        public override bool Equals(object obj)
        {
            if (obj is Range)
            {
                var item = (Range)obj;
                return item.R == R && item.L == L;
            }
            else
                return base.Equals(obj);
        }
        public static Range FromString(string str)
        {
            str = str.Trim();
            var g = str.Split('-');
            Range range = new Range();
            if (g[0] == null || g[0] == "") range.L = long.MinValue; else range.L = long.Parse(g[0]);
            if (g[1] == null || g[1] == "") range.R = long.MinValue; else range.R = long.Parse(g[1]);
            return range;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(L, R);
        }
    }
}
