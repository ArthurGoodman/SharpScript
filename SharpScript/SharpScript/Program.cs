namespace SharpScript {
    public static class Program {
        static void Main(string[] args) {
            Script.RunFile(args.Length > 0 ? args[0] : "../../script.sh");
        }
    }
}
