using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearTarget.Parser
{
    internal class LoadDorks
    {
        public static async Task ReadDorks()
        {
            await Task.Run(() =>
            {
                using (FileStream fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Dorks.txt"), FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        ParserStart.Dorks.Enqueue(line);
                    }
                }
            });
        }
    }
}
