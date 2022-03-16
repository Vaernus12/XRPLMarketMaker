using System;

namespace EMBRS
{
    public static class ConsoleScreen
    {
        public static void ClearConsoleLines(int startingPosition = 7)
        {
            Console.SetCursorPosition(0, startingPosition);
            startingPosition++;
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, startingPosition);
            startingPosition++;
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, startingPosition);
            startingPosition++;
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, startingPosition);
            startingPosition++;
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, startingPosition);
            startingPosition++;
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, startingPosition);
            Console.WriteLine(new string(' ', Console.WindowWidth));
        }

        public static void WriteMessages(params string[] messages)
        {
            int startPos = 7;
            foreach (string s in messages)
            {
                Console.SetCursorPosition(0, startPos);
                Console.WriteLine(s);
                startPos++;
            }
        }

        public static void WriteMessages(int startPos, params string[] messages)
        {
            foreach (string s in messages)
            {
                Console.SetCursorPosition(0, startPos);
                Console.WriteLine(s);
                startPos++;
            }
        }

        public static void InitScreen(ref Spinner spinner, params string[] messages)
        {
            Console.SetCursorPosition(0, 7);
            spinner = new Spinner(0, 7);
            spinner.Start();
            Console.SetCursorPosition(2, 7);
            foreach (string s in messages)
            {
                Console.WriteLine(s);
            }
        }

        public static void InitScreen(params string[] messages)
        {
            Console.SetCursorPosition(2, 7);
            foreach (string s in messages)
            {
                Console.WriteLine(s);
            }
        }

        public static void Stop(ref Spinner spinner)
        {
            spinner.Stop();
        }

        public static void WriteErrors(params string[] messages)
        {
            int startPos = 7;
            foreach (string s in messages)
            {
                Console.SetCursorPosition(0, startPos);
                Console.WriteLine(s);
                startPos++;
            }
            Console.ReadLine();
        }

    }
}
