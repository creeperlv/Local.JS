using System;
using System.IO;
using System.Reflection;
using Terminal.Gui;

namespace Local.JS.Extension.TerminalGUI
{
    public static class GUICS
    {
        static GUICS()
        {
            var asm=AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in asm)
            {
                try
                {
                    if (item.GetType("Terminal.Gui.Application") is not null)
                    {
                        return;
                    }
                }
                catch (Exception)
                {

                }
            }
            var f = new FileInfo(typeof(GUICS).Assembly.Location);
            Assembly.LoadFrom(Path.Combine(f.Directory.FullName, "Terminal.Gui.dll"));
        }
        static Toplevel CurrentTopLevel;
        public static void Init()
        {
            Application.Init();
            CurrentTopLevel = Application.Top;
        }
        public static void Run() => Application.Run();
        public static Toplevel GetTop() => CurrentTopLevel;
        public static Window CreateNewWindow(string Title,int X,int Y,Dim Width,Dim Height)
        {
            Window window = new Window();
            window.Title = Title;
            window.X = X;
            window.Y = Y;
            window.Width = Width;
            window.Height=Height;
            return window;
        }

    }
}
