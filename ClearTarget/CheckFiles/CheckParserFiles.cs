using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearTarget.CheckFiles
{
    internal class CheckParserFiles
    {
        public static void CheckParserInputFiles()
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files"));
            }
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Dorks.txt")))
            {
                File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Dorks.txt"));
            }
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Proxies.txt")))
            {
                File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Proxies.txt"));
            }
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Apis.txt")))
            {
                File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Apis.txt"));
            }
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "GoogleCESApis.txt")))
            {
                File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "GoogleCESApis.txt"));
            }
        }


    }
}
