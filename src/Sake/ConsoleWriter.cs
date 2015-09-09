using System;
using System.IO;
using System.Text;

namespace Sake
{
    internal class ConsoleWriter : TextWriter
    {
        private readonly object _consoleLock = new object();

        public override Encoding Encoding
        {
            get { return Console.OutputEncoding; }
        }

        public override void Write(char[] buffer, int offset, int count)
        {
            var value = new string(buffer, offset, count);
            var index = 0;

            lock (_consoleLock)
            {
                var originalColor = Console.ForegroundColor;

                try
                {
                    for (;;)
                    {
                        var next = value.IndexOf("\x1b-", index);
                        if (next == -1)
                        {
                            Console.Write(value.Substring(index));
                            return;
                        }
                        Console.Write(value.Substring(index, next - index));
                        index = next + 3;
                        Console.ForegroundColor = (ConsoleColor)value[next + 2];
                    }
                }
                finally
                {
                    Console.ForegroundColor = originalColor;
                }
            }
        }
    }
}