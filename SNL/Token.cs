
namespace SNL {
    internal class Token {
        public int Row { get; }
        public int Col { get; }
        public 终结符 Terminal { get; }
        public Token(int line, 终结符 terminal, int col = 0) {
            Row = line;
            Terminal = terminal;
            Col = col;
        }
        public Token(int line, 终结符.TypeEnum terminalType, string content, int col = 0) {
            Row = line;
            Terminal = new 终结符(terminalType, content);
            Col = col;
        }
        public override string ToString() {
            return $"{{{Row},{Col}\t{Terminal.Type}\t{(Terminal.Content.Length < 7 ? Terminal.Content + '\t' : Terminal.Content)}\t}}";
        }
    }
}
