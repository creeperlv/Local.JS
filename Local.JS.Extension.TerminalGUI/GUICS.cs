using System;
using System.IO;
using System.Reflection;
using Terminal.Gui;

namespace Local.JS.Extension.TerminalGUI
{
    /// <summary>
    /// This class was designed for reducing the classes needed to be exposed to JS context as varibales.
    /// </summary>
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
        public static Label CreateLabel(string Content, Pos X, Pos Y)
        {
            return CreateLabel(Content, X, Y, null, null);
        }
        public static Label CreateLabel(string Content, int X, int Y, Dim W, Dim H)
        {

            return CreateLabel(Content, Pos.At(X), Pos.At(Y), W, H);
        }
        public static Label CreateLabel(string Content,Pos X,Pos Y, Dim W,Dim H)
        {

            Label lbl = new Label();
            lbl.Text = Content;
            lbl.X = X;
            lbl.Y = Y;
            if (W is not null) lbl.Width = W;
            if (H is not null) lbl.Height = H;
            return lbl;
        }
        public static Label CreateLabel(string Content, int X, int Y)
        {
            
            return CreateLabel(Content, Pos.At(X), Pos.At(Y));
        }
        public static Button CreateButton(string Content, int X, int Y, Dim W, Dim H, Delegate act)
        {
            return CreateButton(Content,Pos.At(X),Pos.At(Y),W,H,act);
        }
        public static Button CreateButton(string Content,Pos X,Pos Y,Dim W, Dim H,Delegate act)
        {
            Button button = new Button();
            button.Text = Content;
            if(W is not null)
            button.Width = W;
            if(H is not null)
            button.Height = H;
            button.X = X;
            button.Y = Y;
            if (act is not null) button.Clicked += ()=> { 
                act.DynamicInvoke(null, null);
            };
            return button;
        }
    }
}
