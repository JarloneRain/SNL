using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNL {
    internal static class 词法 {
        public const int NUMOFRESERVED = 22;
        public const int NUMOFSYMBOLS = 20;
        public const int RES = 0;
        public const int ID = 1;
        public const int NUM = 2;
        public const int SYM = 3;
        public const int STR = 4;
        public const int PLUS = 100;// +
        public const int SUB = 101;// - 
        public const int MUL = 102;// * 
        public const int DIV = 103;// /
        public const int LT = 104;// <
        public const int LBRACK = 105;// (
        public const int RBRACK = 106;// )
        public const int LSQUBRACK = 107;// [
        public const int RSQUBRACK = 108;// ]
        public const int POINT = 109;// .
        public const int SEMI = 110;// ;
        public const int LBRACE = 111;// {
        public const int RBRACE = 112;// }
        public const int EOFF = 113;// END
        public const int BLANK = 114;// BLANK
        public const int QUO = 115;// \'
        public const int EQU = 116;// =
        public const int INDEX = 117;// ..
        public const int ASSI = 118;// :=
        public const int COM = 119;// 
        internal static readonly string[] Keywords = new string[]
        {
            "BEGIN","END","PROGRAM","VAR",
            "TYPE","PROCEDURE","WHILE","ENDWH",
            "INTEGER","CHAR","ARRAY","OF",
            "INTC","RECORD","IF","THEN",
            "ELSE","FI","DO","WRITE",
            "READ","RETURN"
        };
        internal static readonly string[] Symbolwords = new string[]
        {
            "+","-","*","/","<",
            "(",")","[","]",".",
            ";","{","}","~","BLANK",
            "\'","=","..",":=",","
        };

        static int currentline = 1;
        static int currentchar = 0;

        static int reservedLookup(string str) {
            string Str = str.ToUpper();
            for (int i = 0; i < NUMOFRESERVED; i++) {
                if (String.Compare(Str, Keywords[i]) == 0) return i;
            }
            return -1;
        }//判断是否为保留字，并返回是哪一个保留字，否则返回-1

        static void Init() {
            currentline = 1;
            currentchar = 0;
        }
        static Token Scan(string sourceCode) {//scan函数会扫描出下一个token序列并返回
            char[] symbols = new char[] { '+', '-', '*', '/', '<', '(', ')', '[', ']', ';', '=', ',' };
            int[] symbols_n = new int[] { PLUS, SUB, MUL, DIV, LT, LBRACK, RBRACK, LSQUBRACK, RSQUBRACK, SEMI, EQU, COM };
            //上下一一对应
            //Token t;
            //t.Line = currentline;//从第一行开始
            int lNum = currentline;
            string str = "";
            char c = sourceCode[currentchar++];
        LS0:
            if ('a' <= c && c <= 'z') goto LS1;
            if (c >= 'A' && c <= 'Z') goto LS1;
            if (c >= '0' && c <= '9') goto LS2;
            int indexofsym;
            for (indexofsym = 0; indexofsym < 12; indexofsym++) {
                if (c == symbols[indexofsym]) {//只要当前的字符在符号表中就转向LS3
                    goto LS3;
                }
            }
            if (c == '.') goto LS4;//若出现第一个.则转向ls4识别..
            if (c == '{') goto LS5;//处理注释
            if (c == ':') goto LS6;//处理赋值
            if (c == '\'') goto LS7;//处理字符串
            if (c == '\n') goto LS8;
            if (c == '~') goto LS9;
            if ((c + "").Trim() == String.Empty//判断是否为空白字符
                                               //c[0] == '\t' || c[0] == ' ' || c[0]=='\r'
                ) goto LS10;
            goto OTHER;
        LS1:
            str = str + c;//str代表已经处理了的序列，c【0】永远是当前要处理的字符
            c = sourceCode[currentchar++];
            if (c >= 'a' && c <= 'z') goto LS1;
            else if (c >= 'A' && c <= 'Z') goto LS1;
            else if (c >= '0' && c <= '9') goto LS1;
            currentchar--;//此时一个读完一个串并且读入了一个多余的字符，所以回退一位
            int nres = reservedLookup(str);
            if (nres != -1) {//将已经处理的序列与保留字对比，并识别是哪一个保留字
                Token t1 = new Token(lNum, 终结符.TypeEnum.KW, Keywords[nres]);

                return t1;//识别为保留字
            } else {
                Token t1 = new Token(lNum, 终结符.TypeEnum.ID, str);
                return t1;//识别为标识符
            }
        LS2:
            str = str + c;
            c = sourceCode[currentchar++];
            if (c >= '0' && c <= '9') goto LS2;
            currentchar--;//直到当前字符不是数字才进入此语句，并回退一位
            //t.id = NUM;
            Token t2 = new Token(lNum, 终结符.TypeEnum.NM, str);
            //t.lex = addNUMTable(str_to_num(str));//将这个数字写入数字表并返回位置
            return t2;//识别为NUM
        LS3:
            int lex = symbols_n[indexofsym];
            Token t3 = new Token(lNum, 终结符.TypeEnum.SY, Symbolwords[lex - 100]);
            return t3;//识别为12个单目符，其他符号要么可能为多字符要么有报错处理
        LS4://用来识别 ..、.
            str = str + c;
            c = sourceCode[currentchar++];//读下一个字符
            if (c == '.') {
                Token t4 = new Token(lNum, 终结符.TypeEnum.SY, Symbolwords[INDEX - 100]);
                return t4;//识别..成功
            } else {
                Token t4 = new Token(lNum, 终结符.TypeEnum.SY, Symbolwords[POINT - 100]);
                currentchar--;
                return t4;//识别.成功
            }
        LS5:
            c = sourceCode[currentchar++];
            while (c != '~' && c != '}') {//直到找到文件结束或是右大括号，中间的注释全部忽略

                if (c == '\n' || c == '\r') {
                    currentline++;
                }//如果有换行和回车代表行数加一
                c = sourceCode[currentchar++];
            }
            if (c == '}') {
                c = sourceCode[currentchar++];
                goto LS0;
            } else {
                throw new Exception($"位于第{currentline}行\n注释出错");
            }
        LS6:
            str = str + c;
            c = sourceCode[currentchar++];
            if (c == '=') {
                Token t6 = new Token(lNum, 终结符.TypeEnum.SY, Symbolwords[ASSI - 100]);
                return t6;
            } else {
                throw new Exception($"位于第{currentline}行\n符号处理出错");
            }
        LS7:
            c = sourceCode[currentchar++];
            while (c != '~' && c != '\'') {
                str = str + c;//str中最后就是两个单引号中间的字符串
                c = sourceCode[currentchar++];
            }
            if (c == '\'') {
                Token t7 = new Token(lNum, 终结符.TypeEnum.CH, str);
                return t7;
            } else {
                //showError(STR_ERROR, "");//否则报错
                throw new Exception($"位于第{currentline}行\nsometing wrong : STR");
            }
        LS8://换行符不处理，只把行号加一
            Token t8 = new Token(-1, 终结符.TypeEnum.CH, "");
            currentline++;
            return t8;//
        LS9://文件结束符
            Token t9 = new Token(lNum, 终结符.TypeEnum.SY, Symbolwords[EOFF - 100]);
            return t9;//
        LS10:
            Token t10 = new Token(-1, 终结符.TypeEnum.CH, "");
            return t10;
        OTHER:
            throw new Exception($"位于第{currentline}行\n出现了无法识别的字符：{((byte)c)}");
        }
        internal static List<Token> 分析(string sourceCode) {
            Init();
            List<Token> tokenList = new();
            Token temp = Scan(sourceCode);
            while (!(temp.Terminal.Is(终结符.EOF))) {//只要文件还没结束
                if (temp.Line != -1) {//换行符不生成token序列
                    tokenList.Add(temp);//结果存入tokens
                    //temp.Terminal.Is(终结符.END);
                    if (temp.Terminal.Is(终结符.END)) {//end后面的点不做处理
                        char c = sourceCode[currentchar++];
                        if (c == '.') {
                            break;
                        } else {
                            currentchar--;
                        }
                    }
                }
                temp = Scan(sourceCode);
            }

            return tokenList;
        }
    }

}
