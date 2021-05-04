using System;
using Local.JS.Preprocessor;

namespace Local.JS
{
    public class VersionTool
    {
        static JSInfo currentInfo=new JSInfo();
        public static void SetInfo(JSInfo info)
        {
            currentInfo = info;
        }
        public static Version GetVersion(string Name)
        {
            if (currentInfo.Name.ToUpper() == Name.ToUpper())
            {
                return currentInfo.Version;
            }
            foreach (var item in currentInfo.SubModules)
            {

                if (item.Name.ToUpper() == Name.ToUpper())
                {
                    return item.Version;
                }
            }
            return null;
        }
    }
}
