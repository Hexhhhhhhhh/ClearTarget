using Amib.Threading;
using ClearTarget.Helper;
using ClearTarget.ReadConfigs;
using ClearTarget.UI;
using Newtonsoft.Json.Linq;
using RuriLib.Http;
using RuriLib.Http.Models;
using RuriLib.Proxies;
using RuriLib.Proxies.Clients;
using Spectre.Console;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace ClearTarget.Parser
{
    internal class ParserStart
    {

        public static ConcurrentQueue<string> Dorks = new ConcurrentQueue<string>();
        public static List<string> Proxies = new List<string>();
        public static ConcurrentQueue<string> HQProxies = new ConcurrentQueue<string>();
        public static bool EnableProxyApis = false;
        public static int Threads = 200;
        public static int Timeout = 5000;
        public static int Page = 10;
        public static int UpdateProxiesTime = 300;
        public static string ProxyType = "http";
        public static string Engine = "NO";
        public static bool EnableCUI = false;
        public static bool EnableLogs = true;

        public static int googleMaxPages = 10;

        public static int TotalUrls = 0;
        public static int Filtered = 0;
        public static int Process = 0;
        public static int FUPM = 0;
        public static int UPM = 0;
        public static int DPM = 0;
        public static int RPM = 0;
        public static int _FilteredUrls = 0;
        public static int _TotalUrls = 0;
        public static int _Process = 0;
        public static int _Requests = 0;
        public static int Errors = 0;
        public static int Requests = 0;


        private static string fileTime = null;
        private static readonly object fileLock = new object();
        private static readonly ThreadLocal<Random> threadLocalRandom = new ThreadLocal<Random>(() => new Random());
        private static int n = 0;
        private static Timer _timer;
        private static Timer _timer1;
        private static int _startY = 4;
        private static Stopwatch stopwatch = new Stopwatch();
        private static void TimedEvent(Object o)
        {
            FUPM = (Filtered - _FilteredUrls) * 60;
            _FilteredUrls = Filtered;

            UPM = (TotalUrls - _TotalUrls) * 60;
            _TotalUrls = TotalUrls;

            DPM = (Process - _Process) * 60;
            _Process = Process;

            RPM = (Requests - _Requests) * 60;
            _Requests = Requests;
        }
        private static void TimedEvent1(object o)
        {
            UpdateCUI();
        }
        public static void UpdateCUI()
        {
            Console.Clear();
            MainEntrance.Logo();
            Console.WriteLine("Engine:" + Engine);
            var table = new Table();
            table.AddColumn("Overview");
            table.AddColumn("Value");
            table.AddColumn("Speed");
            table.AddColumn("Value");
            table.AddRow("Filtered Urls", Filtered.ToString(), "FUPM", FUPM.ToString());
            table.AddRow("Total Urls", TotalUrls.ToString(), "UPM", UPM.ToString());
            table.AddRow("Parsed Dorks", Process.ToString(), "DPM", DPM.ToString());
            table.AddRow("Requests", Requests.ToString(),"RPM", RPM.ToString());
            table.AddRow("Errors", Errors.ToString(),"\\","\\");
            var table1 = new Table();
            table1.AddColumn("Setting");
            table1.AddColumn("Value");
            table1.AddRow("Threads", Threads.ToString());
            table1.AddRow("Timeout(ms)", Timeout.ToString());
            table1.AddRow("Pages", Page.ToString());
            AnsiConsole.Write(table);
            Console.WriteLine();
            AnsiConsole.Write(table1);

        }
        private static void UpdateTitle()
        {
            TimeSpan elapsedTime = stopwatch.Elapsed;

            string time = string.Format("{0:00}:{1:00}:{2:00}",
            elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
            Console.Title = $"Clear Target {Program.Version} [Parser {time}] FilteredUrls[" + Filtered + "] FUPM[" + FUPM
                + "] TotalUrls[" + TotalUrls + "] UPM[" + UPM + "] Requests[" + Requests + "] RPM[" + RPM + "] Dorks[" +
                Process + "/" + n + "] DPM[" + DPM + "] Errors[" + Errors + "]";
        }
        public static async Task Initialization()
        {
            ReadParserConfig.Read();
            ChoiceEngine();
            await LoadDorks.ReadDorks();
            if (EnableProxyApis)
            {
                await LoadProxies.ReadApis();
            }
            else
            {
                LoadProxies.ReadProxies();
            }
            Console.Clear();
            MainEntrance.Logo();
            _timer = new Timer(TimedEvent, null, 0, 1000);
            if (EnableCUI)
            {
                _timer1 = new Timer(TimedEvent1, null, 0, 6500);
            }
            DateTime now = DateTime.Now;
            fileTime = now.ToString("yyyy-MM-dd_HH-mm-ss");
            SmartThreadPool smartThreadPool = new SmartThreadPool(new STPStartInfo
            {
                MinWorkerThreads = Threads,
                MaxWorkerThreads = Threads + 100
            });

            stopwatch.Start();
            n = Dorks.Count();
            var tasks = new List<IWorkItemResult<Task>>();
            for (int i = 0; i < Threads; i++)
            {
                var workItemResult = smartThreadPool.QueueWorkItem(async () =>
                {
                    while (Dorks.Count > 0)
                    {
                        if (Dorks.TryDequeue(out var dork))
                        {
                            await StartParsing(dork);
                        }
                    }
                });
                tasks.Add(workItemResult);
            }

            await Task.WhenAll(tasks.Select(result => result.Result));
            stopwatch.Stop();
            smartThreadPool.WaitForIdle();
            smartThreadPool.Shutdown();
            await Console.Out.WriteLineAsync("Parser Work Completion!");
            Thread.Sleep(10000);
            MainEntrance.MainShow();
        }
        public static async void ChoiceEngine()
        {
            Console.Clear();
            MainEntrance.Logo();
            Console.Title = $"Clear Target {Program.Version} [Parser]";
            Console.Write("[");
            MainEntrance.WriteColorWithoutNewLine("Parser Function", ConsoleColor.White);
            Console.WriteLine("]");
            MainEntrance.WriteNumber(1);
            MainEntrance.WriteColor("Google CES(Custom Search Engine)", ConsoleColor.White);
            Console.WriteLine();

            MainEntrance.WriteNumber(19450902);
            MainEntrance.WriteColor("Fucking Japanese([99] Return)", ConsoleColor.Red);
            MainEntrance.WriteColorWithoutNewLine(">>", ConsoleColor.White);
            int i = int.Parse(Console.ReadLine());
            if (i == 1)
            {
                Engine = "Google CSE(Custom Search Engine)";
                Page = googleMaxPages;
                await ParserHelper.ReadGoogleCESApis();
            }
            else if (i == 99)
            {
                MainEntrance.MainShow();
            }
            else if (i == 19450902)
            {
                MainEntrance.MainShow();
            }
            else
            {
                MainEntrance.WriteColor("Error Input!", ConsoleColor.Red);
                Thread.Sleep(2000);
                Initialization();
            }

        }

        public static async Task StartParsing(string dork)
        {
            if (Engine.Equals("Google CSE(Custom Search Engine)"))
            {
                if (ParserHelper.GoogleCSEAndTokens.Count() < ParserHelper.GoogleCESAPIs.Count())
                {
                    Console.Title = $"Clear Target {Program.Version} [Parser] Getting CSE Info... Process[" + ParserHelper.GoogleCSEAndTokens.Count() + "/" + ParserHelper.GoogleCESAPIs.Count() + "]";
                    await GetGoogleCESToken();
                }
                else
                {
                    await GoogleCESEngine(dork);
                }
            }
            else
            {
                await Console.Out.WriteLineAsync("Choice Error Engine!");
            }
        }
        public static RLHttpClient BuildClient()
        {
            string[] proxy;
            if (HQProxies.Count > 0)
            {
                if (HQProxies.TryDequeue(out string p))
                {
                    proxy = p.Split(':');
                }
                else
                {
                    proxy = GetRandomItem(Proxies).Split(":");
                }
            }
            else
            {
                proxy = GetRandomItem(Proxies).Split(":");
            }
            ProxySettings ps;
            if (proxy.Length == 2)
            {
                ps = new ProxySettings()
                {
                    Host = proxy[0],
                    Port = int.Parse(proxy[1]),
                    ConnectTimeout = TimeSpan.FromMicroseconds(Timeout)
                };
            }
            else if (proxy.Length == 3)
            {
                ps = new ProxySettings()
                {
                    Host = proxy[0],
                    Port = int.Parse(proxy[1]),
                    Credentials = new NetworkCredential(proxy[2], proxy[3]),
                    ConnectTimeout = TimeSpan.FromMicroseconds(Timeout)
                };
            }
            else
            {
                return BuildClient();
            }

            var client = new RLHttpClient();
            if (ProxyType.Equals("http"))
            {
                var pc = new HttpProxyClient(ps);
                client = new RLHttpClient(pc);
            }
            else if (ProxyType.Equals("socks4"))
            {
                var pc = new Socks4ProxyClient(ps);
                client = new RLHttpClient(pc);
            }
            else if (ProxyType.Equals("socks5"))
            {
                var pc = new Socks5ProxyClient(ps);
                client = new RLHttpClient(pc);
            }
            else if (ProxyType.Equals("proxyless"))
            {
                var pc = new NoProxyClient(ps);
                client = new RLHttpClient(pc);
            }
            else
            {
                Console.WriteLine("Proxy Type Error!");
            }
            return client;
        }

        public static async Task GetGoogleCESToken()
        {
            try
            {
                var sec = GetRandomItem(ParserHelper.GoogleCESAPIs);
                Uri getUri = new Uri($"https://cse.google.com/cse?oe=utf8&ie=utf8&source=uds&q=dfad&safe=off&sort=&cx={sec}&start=0");
                using var client = BuildClient();
                var userAgent = GetRandomItem(ParserHelper.UserAgentList);
                string response;
                using (var hr = new HttpRequest
                {
                    Uri = getUri,
                    Method = HttpMethod.Get,
                    Headers = new Dictionary<string, string>
                        {
                            { "User-Agent",userAgent },
                        }

                })
                {
                    var r = await client.SendAsync(hr, CancellationToken.None);
                    var r1 = await r.Content.ReadAsStringAsync();
                    response = r1;
                }
                if (response.Contains("relativeUrl="))
                {
                    string u = ExtractStringBetween(response, "relativeUrl='", "';");
                    u = u.Replace("\\x", "%");
                    string u1 = HttpUtility.UrlDecode(u);
                    Uri getToken = new Uri("https://cse.google.com" + u1);
                    using (var hr1 = new HttpRequest
                    {
                        Uri = getToken,
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>
                        {
                            { "User-Agent",userAgent },
                        }
                    })
                    {
                        var v = await client.SendAsync(hr1, CancellationToken.None);
                        var v1 = await v.Content.ReadAsStringAsync();
                        response = v1;
                    }
                    if (response.Contains("\"cse_token\": \""))
                    {
                        string cseToken = ExtractStringBetween(response, "\"cse_token\": \"", "\",");
                        string cselibVersion = ExtractStringBetween(response, "\"cselibVersion\": \"", "\",");
                        ParserHelper.GoogleCSEAndTokens.Add(sec + ":" + cseToken + ":" + cselibVersion);
                    }

                }

            }
            catch (Exception)
            {
            }


        }

        public static async Task GoogleCESEngine(string dork)
        {
            int urls = 0;
            int parsedPages = 0;
            int resultsPerPage = 10;
            using var client = BuildClient();
            for (int i = 0;i<googleMaxPages;i++)
            {
                try
                {
                    string response = null;
                    string[] secInfo = GetRandomItem(ParserHelper.GoogleCSEAndTokens).Split(':');
                    var url = new Uri($"https://cse.google.com/cse/element/v1?rsz=filtered_cse&num=10&source=gcsc&start={(i * resultsPerPage)}&cselibv={secInfo[3]}&cx={secInfo[0]}&q={dork}&safe=off&cse_tok={secInfo[1]}%3A{secInfo[2]}&sort=&exp=cc%2Capo&callback=google.search.cse.api9915&rurl=https%3A%2F%2Fcse.google.com%2Fcse%3Foe%3Dutf8%26ie%3Dutf8%26source%3Duds%26q%3D{dork}%26safe%3Doff%26sort%3D%26cx%{secInfo[0]}%26start%3D0");
                    var userAgent = GetRandomItem(ParserHelper.UserAgentList);

                    using (var hr = new HttpRequest
                    {
                        Uri = url,
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>
                        {
                            { "User-Agent",userAgent },
                        }

                    })
                    {

                        var response1 = await client.SendAsync(hr, CancellationToken.None);
                        var content = await response1.Content.ReadAsStringAsync();
                        response = content;
                    }

                    string cc = response.Replace("/*O_o*/", "").Replace("google.search.cse.api9915(", "").Replace(");", "");
                    if (cc.Contains("\"error\""))
                    {

                        Errors++;
                        Dorks.Enqueue(dork);
                        UpdateTitle();
                        Thread.Sleep(3000);
                        return;
                    }
                    JObject json = JObject.Parse(cc);

                    var items = json["results"];

                    if (items != null)
                    {
                        Requests++;
                        parsedPages++;
                        foreach (var item in items)
                        {

                            if (item["url"] != null)
                            {
                                TotalUrls++;
                                var u = item["url"].ToString();
                                string decodedUrl = HttpUtility.UrlDecode(u);
                                var b = ExtractStringBetween(decodedUrl, "https://", "/");
                                urls++;
                                if (decodedUrl.Contains("?") && decodedUrl.Contains("=") && !ParserHelper.AntipublicList.Contains(b))
                                {
                                    ParserHelper.AntipublicList.Add(b);
                                    Filtered++;
                                    SaveDork("FilteredUrls", decodedUrl);
                                    SaveDork("AllUrls", decodedUrl);
                                }
                                else
                                {
                                    SaveDork("AllUrls", decodedUrl);
                                }
                                UpdateTitle();
                            }
                        }

                    }
                    else
                    {
                        Requests++;
                        WriteToErrorLog(response);
                    }


                }
                catch (Exception ex)
                {

                    if (urls > 0)
                    {
                        Process++;
                        if (EnableLogs)
                        {
                            if (urls > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                await Console.Out.WriteLineAsync("[Parsed]" + dork.Substring(1,20) + "..    Url(s)[" + urls + "]  Page[" + parsedPages + "]");
                                Console.ResetColor();
                            }
                            else
                            {
                                await Console.Out.WriteLineAsync("[Parsed]" + dork.Substring(1, 20) + "..    Url(s)" + urls + "]  Page[" + parsedPages + "]");
                            }
                        }
                        UpdateTitle();

                    }
                    else
                    {
                        Errors++;
                        Dorks.Enqueue(dork);
                        UpdateTitle();
                    }
                    Thread.Sleep(3000);
                    return;
                }
            }
            Process++;
            if (EnableLogs)
            {
                if (urls > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    await Console.Out.WriteLineAsync("[Parsed]" + dork.Substring(1, 20) + "..    Url(s)[" + urls + "]  Page[" + parsedPages + "]");
                    Console.ResetColor();
                }
                else
                {
                    await Console.Out.WriteLineAsync("[Parsed]" + dork.Substring(1, 20) + "..    Url(s)" + urls + "]  Page[" + parsedPages + "]");
                }
            }
            UpdateTitle();


        }
        private static void WriteToErrorLog(string logMessage)
        {
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLog.txt");

            lock (fileLock)
            {
                File.AppendAllText(logFilePath, logMessage);
            }
        }
        public static string ExtractStringBetween(string input, string startMarker, string endMarker)
        {
            string pattern = Regex.Escape(startMarker) + "(.*?)" + Regex.Escape(endMarker);
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }
        private static void SaveDork(string name, string dork)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Result", fileTime);
            string filePath = Path.Combine(path, name + ".txt");
            lock (fileLock)
            {
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(dork);
                }

            }
        }
        public static T GetRandomItem<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentException("The list cannot be null or empty.", nameof(list));
            }

            Random random = threadLocalRandom.Value;
            int index = random.Next(list.Count);
            return list[index];
        }

    }
}
