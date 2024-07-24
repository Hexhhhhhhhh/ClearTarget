using ClearTarget.CheckFiles;
using ClearTarget.Parser;
using ClearTarget.ReadConfigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearTarget.UI
{
    internal class MainEntrance
    {
        public static void WriteColor(string input, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(input);
            Console.ResetColor();
        }
        public static void WriteColorWithoutNewLine(string input, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.Write(input);
            Console.ResetColor();
        }
        public static void Logo()
        {
            WriteColor(""""
                /~`| _  _  _  ~|~ _  _ _  _ _|_
                \_,|(/_(_||    | (_|| (_|(/_ | 
                                       _|      
                Made By Telegram:@B0tt0m1ess
                """", ConsoleColor.Blue);
        }

        public static async Task MainShow()
        {
            Console.Clear();
            Logo();
            CheckParserFiles.CheckParserInputFiles();
            ReadParserConfig.Read();
            Console.Title = $"Clear Target {Program.Version}";
            Console.Write("[");
            WriteColorWithoutNewLine("Clear Target", ConsoleColor.White);
            Console.WriteLine("]");
            WriteNumber(1);
            WriteColor("Parser",ConsoleColor.White);
            WriteColorWithoutNewLine(">>",ConsoleColor.White);
            int i = int.Parse(Console.ReadLine());
            if (i == 1)
            {
                await ParserStart.Initialization();
            }
            else
            {
                WriteColor("Input Error!", ConsoleColor.Red);
                Thread.Sleep(2000);
                MainShow();
            }

        }
        public static void WriteNumber(int n)
        {
            Console.Write("[");
            MainEntrance.WriteColorWithoutNewLine(n.ToString(), ConsoleColor.White);
            Console.Write("]");
        }
    }
}
