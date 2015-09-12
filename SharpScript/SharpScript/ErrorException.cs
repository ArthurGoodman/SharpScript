using System;

namespace SharpScript {
    public class ErrorException : Exception {
        public Position Position { get; set; }

        public ErrorException(string message, Position position)
            : base(message) {
            Position = position;
        }
    }
}
