using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Esprima;
using Esprima.Utils;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using Jint.Runtime.Interop;

namespace Local.JS
{
    public class LocalJSCore
    {
        Engine Engine;
        public LocalJSCore()
        {
            Engine = new Engine();
        }
        public LocalJSCore(params Assembly[] CLRAssemblies)
        {
            Options options = new Options();
            options.AllowClr(CLRAssemblies);
            Engine = new Engine(options);
        }
        StringBuilder Content = new();
        public void AppendProgramSegment(string content)
        {
            Content.Append(content);
        }
        public Engine GetEngine() => Engine;
        public void ClearCurrentProgramSegment()
        {
            Content.Clear();
        }

        [Obsolete]
        public object ObtainLastObject()
        {
            return Engine.GetCompletionValue().ToObject();
        }
        public object Evaluate(string script)
        {
            return Engine.Evaluate(script).ToObject();
        }
        public void Execute()
        {
            Engine.Execute(Content.ToString());
        }
        public JsValue Invoke(string FunctionName,params object[] parameters)
        {
            var result=Engine.Invoke(FunctionName, parameters);
            return result;
        }
        public void ExposeType(string Name, Type t)
        {
            Engine.SetValue(Name, TypeReference.CreateTypeReference(Engine, t));
        }
        public void ExposeObject(string Name, Object obj)
        {
            Engine.SetValue(Name, obj);
        }
        public void ExposeMethod(string Name, Delegate func)
        {
            Engine.SetValue(Name, func);
        }

    }
}
