namespace SharpScript {
    public static class StringExtension {
        public static string SubstringWrapper(this string s, int startIndex, int length) {
            if (length == -1 || startIndex + length > s.Length)
                length = s.Length - startIndex;

            return s.Substring(startIndex, length);
        }
    }
}
