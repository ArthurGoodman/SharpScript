namespace SharpScript {
    public interface IEngine {
        void Run(string source);
        void RunFile(string fileName);
    }
}
