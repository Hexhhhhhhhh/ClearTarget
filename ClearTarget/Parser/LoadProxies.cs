using RuriLib.Http;
using RuriLib.Http.Models;
using RuriLib.Proxies;
using RuriLib.Proxies.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ClearTarget.Parser
{
    internal class LoadProxies
    {
        private static HashSet<string> Microsoft_apis = new HashSet<string>();
        public static HashSet<string> Microsoft_Proxies = new HashSet<string>();
        private static HashSet<string> Microsoft_Proxies2 = new HashSet<string>();
        private static System.Timers.Timer timer;
        public static int updateProxyTimes = 0;

        public static void ReadProxies()
        {
            try
            {
                string[] fileContent = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Proxies.txt"));
                foreach (string line in fileContent)
                {
                    if (line.Contains(':'))
                    {
                        Microsoft_Proxies.Add(line);
                    }
                }
                ParserStart.Proxies = new List<string>(Microsoft_Proxies);
                Microsoft_Proxies.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Read File Error：" + ex.Message);
            }
        }

        public static async Task ReadApis()
        {
            try
            {
                string[] fileContent = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Apis.txt"));
                foreach (string line in fileContent)
                {
                    Microsoft_apis.Add(line);
                }
                await ReadProxiesFromApis();
                Microsoft_Proxies = Microsoft_Proxies2;
                ParserStart.Proxies = new List<string>(Microsoft_Proxies);
                timer = new System.Timers.Timer(ParserStart.UpdateProxiesTime * 1000);
                timer.Elapsed += OnTimedEvent1;
                timer.AutoReset = true;
                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Read File Error：" + ex.Message);
            }
        }

        private static async void OnTimedEvent1(Object source, ElapsedEventArgs e)
        {
            updateProxyTimes++;
            await ReadProxiesFromApis();
            while (Microsoft_Proxies2.Count == 0)
            {
            }
            Microsoft_Proxies = Microsoft_Proxies2;
            ParserStart.Proxies = new List<string>(Microsoft_Proxies);
            Microsoft_Proxies.Clear();
        }

        private static async Task ReadProxiesFromApis()
        {
            Microsoft_Proxies2.Clear();
            foreach (string line in Microsoft_apis)
            {
                try
                {
                    var ps = new ProxySettings();
                    var prc = new NoProxyClient(ps);
                    using var client = new RLHttpClient(prc);
                    using (var hr = new HttpRequest
                    {
                        Uri = new Uri(line),
                        Method = HttpMethod.Get,
                    })
                    {
                        using var response1 = await client.SendAsync(hr);
                        var content = await response1.Content.ReadAsStringAsync();
                        string[] strings = content.Split("\r\n");
                        foreach (string s in strings)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                Microsoft_Proxies2.Add(s);
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Read Proxy API Error" + ex.Message);
                    Thread.Sleep(1000);
                    await ReadProxiesFromApis();
                }
            }
        }
    }
}
