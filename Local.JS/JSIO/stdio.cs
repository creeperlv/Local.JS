using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLUNL.Utilities;

namespace Local.JS.JSIO
{
    public static class stdio
    {
        public static TextReader In;
        public static TextWriter Out;
        public static void SetIn(TextReader sr)
        {
            In = sr;
        }
        public static void SetOut(TextWriter sr)
        {
            Out = sr;
        }
        public static void printfn(string Content, params object[] para)
        {
            Out.Write(String.Format(Content, para));
        }
        public static void printf(string Content, params object[] para)
        {
            Out.Write(Content);
        }
        public static char getc()
        {
            return (char)In.Read();
        }
        public static string gets()
        {
            return In.ReadLine();
        }
        static Queue<string> Input = new Queue<string>();
        public static object[] scanf(string Format)
        {
            var formats = JSIO.Format.FromStirng(Format);
            List<object> objs = new List<object>();

            for (int i = 0; i < formats.Length; i++)
            {
                if (Input.Count == 0) ReadLine();
                var item = formats[i];
                var inp = Input.Dequeue();
                if (item.w != -1)
                {
                    inp = inp.Substring(0, item.w);
                }
                switch (item.type)
                {
                    case FormatT.C:
                        objs.Add(inp.First());
                        break;
                    case FormatT.D:
                        //Console.WriteLine(inp);
                        objs.Add(int.Parse(inp));
                        break;
                    case FormatT.F:
                        objs.Add(Convert.ToSingle(inp));
                        break;
                    case FormatT.I:
                        objs.Add(Convert.ToInt32(inp));
                        break;
                    case FormatT.O:
                        objs.Add(Convert.ToInt32(inp, 8));
                        break;
                    case FormatT.S:
                        objs.Add(inp);
                        break;
                    case FormatT.U:
                        {
                            objs.Add(Convert.ToUInt32(inp));
                        }
                        break;
                    case FormatT.X:
                        {
                            objs.Add(Convert.ToInt32(inp, 16));
                        }
                        break;
                    default:
                        break;
                }
            }
            return objs.ToArray();
        }
        static void ReadLine()
        {
            var a = In.ReadLine();
            var packs = CommandLineTool.Analyze(a);
            foreach (var item in packs.RealParameter)
            {
                //Console.WriteLine("Enq:"+item.EntireArgument);
                Input.Enqueue(item.EntireArgument);
            }
            if (Input.Count == 0) ReadLine();
        }
    }
    internal enum FormatT
    {
        C,// Char
        D,// Decimal 
        F,// Float
        I,// Integer
        O,// Octal
        S,// Stirng
        U,// Unsigned Integer
        X// Hexadecimal
    }
    internal class Format
    {
        internal FormatT type;
        internal int w;
        internal static Format[] FromStirng(string f)
        {
            var L = new List<Format>();
            bool s = false;
            string width = "";
            Format CurrentFormat = new Format();
            for (int i = 0; i < f.Length; i++)
            {
                if (f[i] == '%')
                {
                    width = "";
                    CurrentFormat = new Format();
                    s = true;
                }else
                if (s == true)
                {
                    var item = f[i];
                    if (item is >= '0' and <= '9')
                    {
                        width += item;
                    }
                    else
                    {
                        if (width is not "")
                            CurrentFormat.w = int.Parse(width);
                        else CurrentFormat.w = -1;
                        switch (item)
                        {
                            case 'c':
                                CurrentFormat.type = FormatT.C;
                                break;
                            case 'd':
                                CurrentFormat.type = FormatT.D;
                                break;
                            case 'e':case  'E':
                            case 'f':case  'F':
                            case 'g':case  'G':
                                CurrentFormat.type = FormatT.F;
                                break;
                            case 'i':
                                CurrentFormat.type = FormatT.I;
                                break;
                            case 'o':
                                CurrentFormat.type = FormatT.O;
                                break;
                            case 's':

                                CurrentFormat.type = FormatT.S;
                                break;
                            case 'u':
                                CurrentFormat.type = FormatT.U;
                                break;
                            case 'x':
                            case 'X':
                                CurrentFormat.type = FormatT.X;
                                break;
                            default:
                                break;
                        }
                        s = false;
                        L.Add(CurrentFormat);
                    }
                }
            }
            return L.ToArray(); ;
        }
    }
}
