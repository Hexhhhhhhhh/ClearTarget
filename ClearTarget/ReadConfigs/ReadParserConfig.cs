using ClearTarget.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearTarget.ReadConfigs
{
    internal class ReadParserConfig
    {
        public static void Read()
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Configs"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Configs");
            }
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Configs/ParserConfig.ini"))
            {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/Configs/ParserConfig.ini", """
                 Threads Settings:
                 Threads:200

                 Proxies Settings:
                 Proxies Type(http/socks4/socks5/proxyless):http
                 Enable ProxyApisLoader(T/F):F
                 Update Api's Proxies(s):120
                 Connect Timeout(ms):5000

                 Output Settings:
                 Console Output Mode(CUI/Logs):Logs

                 Google CES Engine Settings:
                 Google Engine Pages(Max 10):10

                 """);
            }
            else
            {
                string[] s = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "/Configs/ParserConfig.ini");
                foreach (string s2 in s)
                {
                    if (s2.Contains(':'))
                    {
                        string[] strings = s2.Split(':');
                        switch (strings[0])
                        {
                            case "Threads":
                                ParserStart.Threads = int.Parse(strings[1]);
                                break;
                            case "Proxies Type(http/socks4/socks5/proxyless)":
                                ParserStart.ProxyType = strings[1];
                                break;
                            case "Update Api's Proxies(s)":
                                ParserStart.UpdateProxiesTime = int.Parse(strings[1]);
                                break;
                            case "Enable ProxyApisLoader(T/F)":
                                if (strings[1].Equals("T"))
                                {
                                    ParserStart.EnableProxyApis = true;
                                }
                                break;
                            case "Console Output Mode(CUI/Logs)":
                                if (strings[1].Equals("CUI"))
                                {
                                    ParserStart.EnableCUI = true;
                                    ParserStart.EnableLogs = false;
                                }
                                break;
                            case "Google Engine Pages(Max 10)":
                                int i = int.Parse(strings[1]);
                                if(i <= 10)
                                {
                                    ParserStart.googleMaxPages = i;
                                }else
                                {
                                    Console.WriteLine("Your page count is greater than 10, automatically set to 10 pages.");
                                }
                                break;
                            case "Connect Timeout(ms)":
                                ParserStart.Timeout = int.Parse(strings[1]);
                                break;
                        }
                    }
                }
            }
        }
    }
}
