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
        public static void printfn(string Content,params object[] para)
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
        public static string[] scanf(string Format)
        {
            var formats = JSIO.Format.FromStirng(Format);

            for (int i = 0; i < formats.Length; i++)
            {
                var a = In.ReadLine();
                var packs = CommandLineTool.Analyze(a);

            }
            return null;
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
        internal string type;
        internal static Format[] FromStirng(string f)
        {
            var L = new List<Format>();
            for (int i = 0; i < f.Length; i++)
            {
                if (f[i] == '%')
                {

                }
            }
            return null;
        }
    }
}
