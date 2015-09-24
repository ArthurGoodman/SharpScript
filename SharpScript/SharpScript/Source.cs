using System;

namespace SharpScript {
    public class Source {
        private string source;

        public Source(string source) {
            this.source = source;
        }

        // TODO: fix this
        public string Quote(Position position) {
            const int maxQuoteLength = 150;

            int start = source.LastIndexOf('\n', position.Offset) + 1;
            int end = source.IndexOf('\n', position.Offset);

            if (end == -1)
                end = source.Length - start;

            string quote = source.Substring(start, end - start);

            int size = quote.Length;

            quote = quote.Substring(Math.Max(0, position.Column - maxQuoteLength / 2), Math.Min(maxQuoteLength, quote.Length));

            string preffix = position.Column > maxQuoteLength / 2 ? "... " : "";
            string suffix = size - position.Column > maxQuoteLength / 2 ? " ..." : "";

            int pos = position.Column > maxQuoteLength / 2 ? maxQuoteLength / 2 : position.Column;

            string pointer = new string(' ', pos + preffix.Length - 1) + "^";

            return preffix + quote + suffix + "\n" + pointer;
        }
    }
}
