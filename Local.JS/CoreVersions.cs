using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Jint;

namespace Local.JS
{
    public class CoreVersions
    {
        public static Version GetCoreVersion()
        {
            return typeof(LocalJSCore).Assembly.GetName().Version;
        }
        public static Version GetRuntimeVersion()
        {
            return Environment.Version;
        }
        public static Version GetEngineVersion()
        {
            return typeof(Engine).Assembly.GetName().Version;
        }
    }
}
