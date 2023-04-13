using System;
using System.Collections.Generic;
using static SNL.语法树;

namespace SNL {
    internal class 语法分析异常 : Exception {
        Token token;

        public override string Message => $"语法分析发生异常，终止于\n{token}";
        public 语法分析异常(Token token) {
            this.token = token;
        }
    }
    internal static class 语法 {
        
        public static 语法树 分析_递归下降(List<Token> tokenList) {
            //写完之前防止报错用的，最后记得改掉jnjknknjk
            return 分析_LL1(tokenList);
        }


        class 表达式构造器 {
            public static readonly Dictionary<char, int> 算符优先级 = new() {
                ['('] = 0,
                ['<'] = 1,
                ['='] = 1,
                ['+'] = 2,
                ['-'] = 2,
                ['*'] = 3,
                ['/'] = 3,
                ['_'] = 4,
                ['.'] = 5,
            };
            Stack<Token> 运算符栈 = new();
            Stack<表达式> 表达式栈 = new();
            语句 亲;
            public 表达式构造器(语句 亲) { this.亲 = 亲; }
            void 构造结点并压栈(Token token) {
                var exp = new 表达式(token.Line, 亲, token.Terminal.Content[0]);
                表达式 右 = 表达式栈.Pop();
                表达式 左 = 表达式栈.Pop();
                exp.左 = new(左.行号, exp, 左.算符, 左.内容) {
                    左 = 左.左,
                    右 = 左.右
                };
                exp.右 = new(右.行号, exp, 右.算符, 右.内容) {
                    左 = 右.左,
                    右 = 右.右
                };
                表达式栈.Push(exp);
            }
            public void Push(Token token) {
                if (false) {
                } else if (token.Terminal.Content[0] == '(') {
                    运算符栈.Push(token);
                } else if (token.Terminal.Content[0] == ')') {
                    for (
                        Token t = 运算符栈.Pop();
                        t.Terminal.Content[0] != '(';
                        t = 运算符栈.Pop()
                    ) {
                        构造结点并压栈(t);
                    }
                } else {
                    while (运算符栈.Count != 0 &&
                        算符优先级[运算符栈.Peek().Terminal.Content[0]] >= 算符优先级[token.Terminal.Content[0]]
                    ) {
                        构造结点并压栈(运算符栈.Pop());
                    }
                    运算符栈.Push(token);
                }
            }
            public void Push(表达式 exp) {
                表达式栈.Push(exp);
            }
            public 表达式 Get() {
                while (运算符栈.Count > 0) {
                    构造结点并压栈(运算符栈.Pop());
                }
                return 表达式栈.Pop();
            }
        }

        public static 语法树 分析_LL1(List<Token> tokenList) {
            List<(
                string 名称, 类型描述 类型
            )> //curVarList = new(),
                curFieldList = new();
            List<语句> curStmList = new();
            bool isArgVar = false;
            bool isArray = false;
            Stack<(结符 语法符号, int 产生式编号)> 分析栈 = new(new (结符 语法符号, int 产生式编号)[] { (非终结符.总体程序, 0) });
            表达式构造器? curExp = null;
            过程声明? curProc = null;
            语句? curStm = null;
            string curID = "";
            int curLine = 0;
            类型描述? curType = null;
            int arrayLow = 0, arrayTop = 0;
            Action<List<语句>, 语句, 表达式构造器?> 当前语句结束时根据当前语句是否为赋值语句来判断是否需要对当前赋值语句插入右值并将当前语句加到语句列表 = (stmList, stm, exp) => {
                if (stm as 赋值语句 != null) {
                    (stm as 赋值语句)!.右 = exp!.Get();
                }
                stmList!.Add(stm!);
            };

            foreach (Token token in tokenList) {
            CONTINUE:
                try {
                    var top = 分析栈.Pop();
                    if (top.语法符号.IsTerminal) {
                        终结符 terminal = (top.语法符号 as 终结符)!;
                        if (!terminal.Matches(token)) {
                            throw new Exception();
                        }
                        switch (top.产生式编号) {
                            case 00://总体程序→PROGRAM ID 声明部分 过程主体
                                if (false) {
                                } else if (terminal.Is(终结符.ID)) {
                                    curProc = new 过程声明(
                                        token.Line,
                                        curProc,
                                        token.Terminal.Content
                                    );
                                    //curVarList = curProc.参数声明列表;
                                }
                                break;
                            case 04://类型声表→ID = 类型定义 ; 类型声余
                                if (false) {
                                } else if (terminal.Is(终结符.ID)) {
                                    curID = token.Terminal.Content;
                                    curLine = token.Line;
                                } else if (terminal.Is(终结符.分号)) {
                                    curProc!.类型声明列表.Add(new 类型声明(
                                        curLine,
                                        curProc,
                                        curID,
                                        curType!
                                    ));
                                }
                                break;
                            case 09://类型定义→ID
                                if (false) {
                                } else if (terminal.Is(终结符.ID)) {
                                    curType = new 自定类型描述(token.Terminal.Content) { 行号 = curLine };
                                }
                                break;
                            case 10://基础类型→INTEFER
                                if (false) {
                                } else if (terminal.Is(终结符.INTEGER)) {
                                    curType = isArray ? new 数组类型描述 {
                                        行号 = curLine,
                                        下界 = arrayLow,
                                        上界 = arrayTop,
                                        元素类型 =基础类型描述.整数类型(token.Line),
                                    } : 基础类型描述.整数类型(token.Line);
                                    isArray = false;
                                }
                                break;
                            case 11://基础类型→CHAR
                                if (false) {
                                } else if (terminal.Is(终结符.CHAR)) {
                                    curType = isArray ? new 数组类型描述 {
                                        行号 = curLine,
                                        下界 = arrayLow,
                                        上界 = arrayTop,
                                        元素类型 = 基础类型描述.字符类型(token.Line),
                                    } : 基础类型描述.字符类型(token.Line);
                                    isArray = false;
                                }
                                break;
                            case 14://数组类型→ARRAY [ 数组下界 .. 数组上界 ] OF　基础类型
                                if (false) {
                                } else if (terminal.Is(终结符.ARRAY)) {
                                    curLine = token.Line;
                                    isArray = true;
                                }
                                break;
                            case 15://数组下界→INTC
                                if (false) {
                                } else if (terminal.Is(终结符.INTC)) {
                                    arrayLow = int.Parse(token.Terminal.Content);
                                }
                                break;
                            case 16://数组上界→INTC
                                if (false) {
                                } else if (terminal.Is(终结符.INTC)) {
                                    arrayTop = int.Parse(token.Terminal.Content);
                                }
                                break;
                            case 17://记录类型→RECORD 域描述表 END
                                if (false) {
                                } else if (terminal.Is(终结符.RECORD)) {
                                    curLine = token.Line;
                                } else if (terminal.Is(终结符.END)) {
                                    curType = new 记录类型描述 {
                                        行号 = curLine,
                                        字段 = curFieldList,
                                    };
                                }
                                break;
                            case 22://字段名表→ID 字段名余
                                if (false) {
                                } else if (terminal.Is(终结符.ID)) {
                                    curFieldList.Add((token.Terminal.Content, curType!));
                                }
                                break;
                            case 30://变量名表→ID 变量名余
                                if (false) {
                                } else if (terminal.Is(终结符.ID)) {
                                    //curVarList.Add((terminal.Content, curType!));
                                    curProc!.变量声明列表.Add(new 变量声明(token.Line, curProc!, token.Terminal.Content, curType!));
                                }
                                break;
                            case 34://过程声明→PROCEDURE ID ( 参数列表 ) ; 声明部分 过程主体 过程声明
                                if (false) {
                                } else if (terminal.Is(终结符.ID)) {
                                    curProc = new 过程声明(token.Line, curProc, token.Terminal.Content);
                                    (curProc.亲 as 过程声明)!.过程声明列表.Add(curProc);
                                    isArgVar = false;
                                }
                                break;
                            case 39://形参更多→; 参数描表
                                if (false) {
                                } else if (terminal.Is(终结符.分号)) {
                                    isArgVar = false;
                                }
                                break;
                            case 41://形式参数→VAR 类型定义 参量名表
                                if (false) {
                                } else if (terminal.Is(终结符.VAR)) {
                                    isArgVar = true;
                                }
                                break;
                            case 42://参量名表→ID 参量名余
                                if (false) {
                                } else if (terminal.Is(终结符.ID)) {
                                    curProc!.参数列表.Add((
                                        new 变量声明(
                                            token.Line,
                                            curProc,
                                            token.Terminal.Content,
                                            curType!
                                        ),
                                        isArgVar
                                    ));
                                }
                                break;
                            case 45://过程主体→BEGIN 语句列表 END
                                if (false) {
                                } else if (terminal.Is(终结符.BEGIN)) {
                                    curStm = new 空的语句(token.Line, curProc!);
                                    curStmList = curProc!.过程体;
                                } else if (terminal.Is(终结符.END)) {
                                    当前语句结束时根据当前语句是否为赋值语句来判断是否需要对当前赋值语句插入右值并将当前语句加到语句列表(curStmList, curStm!, curExp);
                                    curProc = (curProc!.亲 != null) ? (curProc.亲 as 过程声明) : curProc;
                                }
                                break;
                            case 48://语句更多→; 语句列表
                                if (false) {
                                } else if (terminal.Is(终结符.分号)) {
                                    当前语句结束时根据当前语句是否为赋值语句来判断是否需要对当前赋值语句插入右值并将当前语句加到语句列表(curStmList, curStm!, curExp);
                                }
                                break;
                            case 54://一条语句→ID 赋调语句
                                if (false) {
                                } else if (terminal.Is(终结符.ID)) {
                                    curID = token.Terminal.Content;
                                    curLine = token.Line;
                                }
                                break;
                            case 57://赋值语句→变量赘余 := 算术式子
                                if (false) {
                                } else if (terminal.Is(终结符.赋值)) {
                                    (curStm as 赋值语句)!.左 = curExp!.Get();
                                    curExp = new 表达式构造器(curStm!);
                                }
                                break;
                            case 58://条件语句→IF 条件式子 THEN 语句列表 ELSE 语句列表 FI
                                if (false) {
                                } else if (terminal.Is(终结符.IF)) {
                                    curStm = new 条件语句(token.Line, curStm!.亲!);
                                    curExp = new 表达式构造器(curStm!);
                                } else if (terminal.Is(终结符.THEN)) {
                                    (curStm as 条件语句)!.条件 = curExp!.Get();
                                    curStm = new 空的语句(token.Line, curStm!);
                                    curStmList = (curStm!.亲! as 条件语句)!.THEN;
                                } else if (terminal.Is(终结符.ELSE)) {
                                    当前语句结束时根据当前语句是否为赋值语句来判断是否需要对当前赋值语句插入右值并将当前语句加到语句列表(curStmList, curStm!, curExp);
                                    curStm = new 空的语句(token.Line, curStm!.亲!);
                                    curStmList = (curStm!.亲! as 条件语句)!.ELSE;
                                } else if (terminal.Is(终结符.FI)) {
                                    当前语句结束时根据当前语句是否为赋值语句来判断是否需要对当前赋值语句插入右值并将当前语句加到语句列表(curStmList, curStm!, curExp);
                                    curStm = curStm!.亲 as 条件语句;
                                    if (curStm!.亲 as 过程声明 != null) {
                                        curStmList = (curStm!.亲 as 过程声明)!.过程体;
                                    } else {
                                        switch ((curStm!.亲 as 语句)!.语句类别) {
                                            case 语句.语句类别Enum.条件语句:
                                                条件语句 condStm = (curStm!.亲 as 条件语句)!;
                                                curStmList = condStm.ELSE.Count == 0 ? condStm.THEN : condStm.ELSE;
                                                break;
                                            case 语句.语句类别Enum.循环语句:
                                                curStmList = (curStm!.亲 as 循环语句)!.循环体;
                                                break;
                                            default:
                                                throw new Exception("我也不知道是什么错误，反正就是就是出错了！");
                                        }
                                    }
                                }
                                break;
                            case 59://循环语句→WHILE 条件式子 DO 语句列表 ENDWH
                                if (false) {
                                } else if (terminal.Is(终结符.WHILE)) {
                                    curStm = new 循环语句(token.Line, curStm!.亲!);
                                    curExp = new 表达式构造器(curStm!);
                                } else if (terminal.Is(终结符.DO)) {
                                    (curStm as 循环语句)!.条件 = curExp!.Get();
                                    curStmList = (curStm as 循环语句)!.循环体;
                                    curStm = new 空的语句(token.Line, curStm);
                                } else if (terminal.Is(终结符.ENDWH)) {
                                    当前语句结束时根据当前语句是否为赋值语句来判断是否需要对当前赋值语句插入右值并将当前语句加到语句列表(curStmList, curStm!, curExp);
                                    curStm = curStm!.亲 as 循环语句;
                                    if (curStm!.亲 as 过程声明 != null) {
                                        curStmList = (curStm!.亲 as 过程声明)!.过程体;
                                    } else {
                                        switch ((curStm!.亲 as 语句)!.语句类别) {
                                            case 语句.语句类别Enum.条件语句:
                                                条件语句 condStm = (curStm!.亲 as 条件语句)!;
                                                curStmList = condStm.ELSE.Count == 0 ? condStm.THEN : condStm.ELSE;
                                                break;
                                            case 语句.语句类别Enum.循环语句:
                                                curStmList = (curStm!.亲 as 循环语句)!.循环体;
                                                break;
                                            default:
                                                throw new Exception("我也不知道是什么错误，反正就是就是出错了！");
                                        }
                                    }
                                    curStmList.Add(curStm!);
                                    //curStm = curStm.亲 as 语句;
                                }
                                break;
                            case 60://输入语句→READ ( ID )
                                if (false) {
                                } else if (terminal.Is(终结符.READ)) {
                                    curStm = new 输入语句(token.Line, curStm!.亲!);
                                } else if (terminal.Is(终结符.左圆)) {
                                    curExp = new 表达式构造器(curStm!);
                                } else if (terminal.Is(终结符.ID)) {
                                    curExp!.Push(new 表达式(token.Line, curStm!, '$', token.Terminal.Content));
                                } else if (terminal.Is(终结符.右圆)) {
                                    (curStm as 输入语句)!.待输入量 = curExp!.Get();
                                }
                                break;
                            case 61://输出语句→WRITE ( 算术式子 )
                                if (false) {
                                } else if (terminal.Is(终结符.WRITE)) {
                                    curStm = new 输出语句(token.Line, curStm!.亲!);
                                } else if (terminal.Is(终结符.左圆)) {
                                    curExp = new 表达式构造器(curStm!);
                                } else if (terminal.Is(终结符.右圆)) {
                                    (curStm as 输出语句)!.待输出值 = curExp!.Get();
                                }
                                break;
                            case 62://返沪语句→RETURN
                                if (false) {
                                } else if (terminal.Is(终结符.RETURN)) {
                                    curStm = new 返回语句(token.Line, curStm!.亲!);
                                }
                                break;
                            case 63://调用语句→( 实参列表 )
                                if (false) {
                                } else if (terminal.Is(终结符.左圆)) {
                                    curStm = new 调用语句(token.Line, curStm!.亲!, curID);
                                    curExp = new 表达式构造器(curStm!);
                                } else if (terminal.Is(终结符.右圆)) {
                                    if (curExp != null) {
                                        (curStm as 调用语句)!.实参列表.Add(curExp!.Get());
                                    }
                                }
                                break;
                            case 67://实参更多→, 实参列表
                                if (false) {
                                } else if (terminal.Is(终结符.逗号)) {
                                    (curStm as 调用语句)!.实参列表.Add(curExp!.Get());
                                    curExp = new 表达式构造器(curStm!);
                                }
                                break;
                            case 75://算术因子→( 算术式子 )
                                if (false) {
                                } else if (terminal.Is(终结符.左圆)) {
                                    curExp!.Push(token);
                                } else if (terminal.Is(终结符.右圆)) {
                                    curExp!.Push(token);
                                }
                                break;
                            case 76://算术因子→INTC
                                if (false) {
                                } else if (terminal.Is(终结符.INTC)) {
                                    curExp!.Push(new 表达式(token.Line, curStm!, '#', token.Terminal.Content));
                                }
                                break;
                            case 77://算术因子→ID 变量赘余
                                if (false) {
                                } else if (terminal.Is(终结符.ID)) {
                                    curExp!.Push(new 表达式(token.Line, curStm!, '$', token.Terminal.Content));
                                }
                                break;
                            case 79://变量赘余→[ 算术式子 ]
                                if (false) {
                                } else if (terminal.Is(终结符.左方)) {
                                    //curExp!.Push(token); 这里特殊处理一下，把 arr[i]处理成arr_(i)
                                    curExp!.Push(new Token(token.Line, new 终结符(终结符.TypeEnum.SY, "_")));
                                    curExp!.Push(new Token(token.Line, 终结符.左圆));
                                } else if (terminal.Is(终结符.右方)) {
                                    curExp!.Push(new Token(token.Line, 终结符.右圆));
                                }
                                break;
                            case 80://变量赘余→. 记录域名
                                if (false) {
                                } else if (terminal.Is(终结符.单点)) {
                                    curExp!.Push(token);
                                }
                                break;
                            case 81://记录域名→ID 域名更多
                                if (false) {
                                } else if (terminal.Is(终结符.ID)) {
                                    curExp!.Push(new 表达式(token.Line, curStm!, '$', token.Terminal.Content));
                                }
                                break;
                            case 83://域名更多→[ 算术式子 ]
                                if (false) {
                                } else if (terminal.Is(终结符.左方)) {
                                    curExp!.Push(new 表达式(curLine, curStm!, '$', curID));
                                    //curExp!.Push(token); 这里特殊处理一下，把 arr[i]处理成arr_(i)
                                    curExp!.Push(new Token(token.Line, new 终结符(终结符.TypeEnum.SY, "_")));
                                    curExp!.Push(new Token(token.Line, 终结符.左圆));
                                } else if (terminal.Is(终结符.右方)) {
                                    curExp!.Push(new Token(token.Line, 终结符.右圆));
                                }
                                break;
                            case 84://比较符号→<
                                if (false) {
                                } else if (terminal.Is(终结符.小于)) {
                                    curExp!.Push(token);
                                }
                                break;
                            case 85://比较符号→=
                                if (false) {
                                } else if (terminal.Is(终结符.等号)) {
                                    curExp!.Push(token);
                                }
                                break;
                            case 86://加减符号→+
                                if (false) {
                                } else if (terminal.Is(终结符.加号)) {
                                    curExp!.Push(token);
                                }
                                break;
                            case 87://加减符号→-
                                if (false) {
                                } else if (terminal.Is(终结符.减号)) {
                                    curExp!.Push(token);
                                }
                                break;
                            case 88://乘除符号→* 
                                if (false) {
                                } else if (terminal.Is(终结符.乘号)) {
                                    curExp!.Push(token);
                                }
                                break;
                            case 89://乘除符号→/
                                if (false) {
                                } else if (terminal.Is(终结符.除号)) {
                                    curExp!.Push(token);
                                }
                                break;
                        }
                    } else {
                        非终结符 nonTerminal = (top.语法符号 as 非终结符)!;
                        (int 当前语法编号, 结符[] 产生式) = nonTerminal[token.Terminal];
                        for (int k = 产生式.Length - 1; k >= 0; k--) {
                            分析栈.Push((产生式[k], 当前语法编号));
                        }
                        switch (当前语法编号) {
                            case 55://赋调语句→赋值语句
                                curStm = new 赋值语句(curLine, curStm!.亲!);
                                curExp = new 表达式构造器(curStm!);
                                curExp!.Push(new 表达式(curLine, curStm!, '$', curID));
                                break;
                        }
                        goto CONTINUE;
                    }
                } catch (Exception) {
                    throw new 语法分析异常(token);
                }

            }
            return new 语法树(curProc!);
        }

    }
}
