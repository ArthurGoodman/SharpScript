namespace SharpScript {
    public class Position {
        public int Offset { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        public bool Valid { get; private set; }

        public Position() {
            Offset = Line = Column = 0;

            Valid = false;
        }

        public Position(int offset, int line, int column) {
            Offset = offset;
            Line = line;
            Column = column;

            Valid = true;
        }

        public Position Shifted(int delta) {
            return new Position(Offset + delta, Line, Column + delta);
        }

        public override string ToString() {
            return Line.ToString() + ":" + Column.ToString();
        }
    }
}
