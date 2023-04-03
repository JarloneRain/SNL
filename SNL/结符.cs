using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SNL.终结符;

namespace SNL {
    internal abstract class 结符 {
        public bool IsTerminal { get; }
        public string Content { get; }
        public 结符(bool isTerminal, string content) {
            this.IsTerminal = isTerminal;
            this.Content = content;
        }
    }
    internal partial class 终结符 : 结符 {
        public enum TypeEnum {
            KW,//keyword
            SY,//symbol
            CH,//char
            NM,//number
            ID,//id
        }
        public TypeEnum Type { get; }
        public 终结符(
            TypeEnum terminalType,
            string content = ""
        ) : base(true, content) {
            this.Type = terminalType;
        }
        public static 终结符 Id(string content = "") {
            return new 终结符(TypeEnum.ID, content);
        }
        public string KeyStr {
            get {
                switch (this.Type) {
                    case TypeEnum.KW:
                    case TypeEnum.SY:
                        return this.Type.ToString() + this.Content;
                    case TypeEnum.CH:
                    case TypeEnum.NM:
                    case TypeEnum.ID:
                        return this.Type.ToString();
                }
                return "";
            }
        }

        public bool Matches(Token token) {
            return this.Is(token.Terminal);
        }
        public bool Is(终结符 that) {
            return this.KeyStr == that.KeyStr;
        }

    }

    internal partial class 非终结符 : 结符 {
        public 非终结符(string content) : base(false, content) { }
        public (int 产生式编号, 结符[] 产生式) this[终结符 terminal] {
            get {
                int 编号 = Predict[this.Content][terminal.KeyStr];
                return (编号,产生式[编号]);
            }
        }
    }
}
