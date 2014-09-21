using System;
using System.Diagnostics;

namespace Playr
{
    public static class Log
    {
        static readonly object consoleLock = new object();

        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args)
        {
            Debug(String.Format(format, args));
        }

        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            lock (consoleLock)
                using (SetForegroundColor(ConsoleColor.DarkGray))
                    Console.WriteLine("DEBUG: {0}", message);
        }

        public static void Error(string format, params object[] args)
        {
            Error(String.Format(format, args));
        }

        public static void Error(Exception ex)
        {
            Error(ex.ToString());
        }

        public static void Error(string message)
        {
            lock (consoleLock)
                using (SetForegroundColor(ConsoleColor.Red))
                    Console.WriteLine("{0}", message);
        }

        public static void Info(string format, params object[] args)
        {
            Info(String.Format(format, args));
        }

        public static void Info(string message)
        {
            lock (consoleLock)
                Console.WriteLine("{0}", message);
        }

        public static void Message(string format, params object[] args)
        {
            Message(String.Format(format, args));
        }

        public static void Message(string message)
        {
            lock (consoleLock)
                using (SetForegroundColor(ConsoleColor.DarkGray))
                    Console.WriteLine("{0}", message);
        }

        public static void Warning(string format, params object[] args)
        {
            Warning(String.Format(format, args));
        }

        public static void Warning(string message)
        {
            lock (consoleLock)
                using (SetForegroundColor(ConsoleColor.Yellow))
                    Console.WriteLine("{0}", message);
        }

        // Helpers

        static IDisposable SetForegroundColor(ConsoleColor foregroundColor)
        {
            var existingColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            return new DisposeWrapper(() => Console.ForegroundColor = existingColor);
        }

        class DisposeWrapper : IDisposable
        {
            readonly Action disposer;

            public DisposeWrapper(Action disposer)
            {
                this.disposer = disposer;
            }

            public void Dispose()
            {
                disposer();
            }
        }
    }
}
