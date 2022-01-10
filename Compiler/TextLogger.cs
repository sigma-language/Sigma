namespace Compiler
{
    using System;
    using System.IO;

    public class TextLogger
    {
        private TextWriter writer;

        public TextLogger(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Abort(string message)
        {
            this.writer.WriteLine(message);
            Environment.Exit(1);
        }
    }
}
