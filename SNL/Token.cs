using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNL {
    internal class Token {
        public int Line { get; }
        public 终结符 Terminal { get; }
        public Token(int line, 终结符 terminal) {
            this.Line = line;
            this.Terminal = terminal;
        }
        public Token(
            int line,
            终结符.TypeEnum terminalType,
            string content
        ) {
            this.Line = line;
            this.Terminal = new 终结符(terminalType,content);
        }
        public override string ToString() {
            return $"{{\t{Line}\t{Terminal.Type}\t{(Terminal.Content.Length<7?Terminal.Content+'\t':Terminal.Content)}\t}}";
        }
    }
}
