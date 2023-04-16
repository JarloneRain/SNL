using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using static SNL.语法树;

namespace SNL {
    internal class 语法分析异常 : Exception {
        Token token;

        public override string Message => $"语法分析发生异常，终止于\n{token}";
        public 语法分析异常(Token token) {
            this.token = token;
        }
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
        表达式? 表达式更新亲结点(表达式? exp, 语法点 亲斤) {
            if (exp == null) { return null; }
            var newexp = new 表达式(exp.行号, 亲斤, exp.算符, exp.内容);
            newexp.左 = 表达式更新亲结点(exp.左, newexp);
            newexp.右 = 表达式更新亲结点(exp.右, newexp);
            return newexp;
        }
        void 构造结点并压栈(Token token) => 表达式栈.Push(表达式更新亲结点(new 表达式(token.Line, 亲, token.Terminal.Content[0]) {
            右 = 表达式栈.Pop(),
            左 = 表达式栈.Pop(),
        }, 亲)!);

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

    internal static class 语法 {

        static int _Index = 0;
        static List<Token> _TokenList = new();
        static List<语句> stmlist = new();
        static 语法点? 处理(this 非终结符 nonTerminal, 语法点? 亲) {
            return 递归下降![nonTerminal.Content][nonTerminal[_TokenList[_Index].Terminal].产生式编号](亲);
        }
        static 表达式构造器? _exp;
        static Dictionary<string,//终结符
            Dictionary<
                int,//语法编号
                Func<语法点?, 语法点?>//操作
                >
            > 递归下降 = new() {
                [非终结符.总体程序.Content] = new() {
                    [0] = (亲) => {//PROGRAM ID 声明部分 过程主体
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.PROGRAM.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;
                        token = _TokenList[_Index];
                        if (!终结符.ID.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var program = new 过程声明(token.Line, 亲, token.Terminal.Content);
                        _Index++;
                        非终结符.声明部分.处理(program);
                        非终结符.过程主体.处理(program);
                        return program;
                    },
                },
                [非终结符.声明部分.Content] = new() {
                    [1] = (亲) => {//类型声明 变量声明 过程声明
                        非终结符.类型声明.处理(亲);
                        非终结符.变量声明.处理(亲);
                        非终结符.过程声明.处理(亲);
                        return null;
                    },
                },
                [非终结符.类型声明.Content] = new() {
                    [2] = (亲) => {//
                        return null;
                    },
                    [3] = (亲) => {//TYPE 类型声表
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.TYPE.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;
                        非终结符.类型声表.处理(亲);
                        return null;
                    },
                },
                [非终结符.类型声表.Content] = new() {
                    [4] = (亲) => {//ID = 类型定义 ; 类型声余
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.ID.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var typename = token.Terminal.Content;
                        _Index++;

                        token = _TokenList[_Index];
                        if (!终结符.等号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var line = token.Line;
                        _Index++;

                        var typedec = (非终结符.类型定义.处理(亲) as 类型声明)!;/////////

                        token = _TokenList[_Index];
                        if (!终结符.分号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        (亲 as 过程声明)!.类型声明列表.Add(new 类型声明(line, 亲!, typename, typedec.类型定义));///////
                        非终结符.类型声余.处理(亲);

                        return null;
                    },
                },
                [非终结符.类型声余.Content] = new() {
                    [5] = (亲) => {//
                        return null;
                    },
                    [6] = (亲) => {//类型声表
                        非终结符.类型声表.处理(亲);
                        return null;
                    },
                },
                [非终结符.类型定义.Content] = new() {
                    [7] = (亲) => {//基础类型
                        return 非终结符.基础类型.处理(亲);
                    },
                    [8] = (亲) => {//结构类型
                        return 非终结符.结构类型.处理(亲);
                    },
                    [9] = (亲) => {//ID
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.Id("类型名").Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;
                        return new 类型声明(token.Line, 亲!, "", new 自定类型描述(token.Line, token.Terminal.Content));
                    },
                },
                [非终结符.基础类型.Content] = new() {
                    [10] = (亲) => {//INTEGER
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.INTEGER.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;
                        return new 类型声明(token.Line, 亲!, "", 基础类型描述.整数类型(token.Line));
                    },
                    [11] = (亲) => {//CHAR
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.CHAR.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;
                        return new 类型声明(token.Line, 亲!, "", 基础类型描述.字符类型(token.Line));
                    },
                },
                [非终结符.结构类型.Content] = new() {
                    [12] = (亲) => {//数组类型 
                        return 非终结符.数组类型.处理(亲);
                    },
                    [13] = (亲) => {//记录类型
                        return 非终结符.记录类型.处理(亲);
                    },
                },
                [非终结符.数组类型.Content] = new() {
                    [14] = (亲) => {//ARRAY [ 数组下界 .. 数组上界 ] OF 基础类型
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.ARRAY.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var line = token.Line;
                        _Index++;

                        token = _TokenList[_Index];
                        if (!终结符.左方.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        var low = int.Parse((非终结符.数组下界.处理(亲) as 表达式)!.内容);

                        token = _TokenList[_Index];
                        if (!终结符.双点.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        var top = int.Parse((非终结符.数组上界.处理(亲) as 表达式)!.内容);

                        token = _TokenList[_Index];
                        if (!终结符.右方.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        token = _TokenList[_Index];
                        if (!终结符.OF.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        var basetype = ((非终结符.基础类型.处理(亲) as 类型声明)!.类型定义 as 基础类型描述)!;

                        return new 类型声明(0, 亲!, "", new 数组类型描述(line, low, top, basetype));
                    },
                },
                [非终结符.数组下界.Content] = new() {
                    [15] = (亲) => {//INTC
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.INTC.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;
                        return new 表达式(token.Line, 亲!, '#', token.Terminal.Content);
                    },
                },
                [非终结符.数组上界.Content] = new() {
                    [16] = (亲) => {//INTC
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.INTC.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;
                        return new 表达式(token.Line, 亲!, '#', token.Terminal.Content);
                    },
                },
                [非终结符.记录类型.Content] = new() {
                    [17] = (亲) => {//RECORD 域描述表 END
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.RECORD.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var line = token.Line;
                        _Index++;

                        var record = new 类型声明(0, 亲!, "", new 记录类型描述(line));

                        非终结符.域描述表.处理(record);

                        token = _TokenList[_Index];
                        if (!终结符.END.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        (record.类型定义 as 记录类型描述)!.字段.RemoveAll(f => f.名称 == "");
                        return record;
                    },
                },
                [非终结符.域描述表.Content] = new() {
                    [18] = (亲) => {//基础类型 字段名表 ; 域描述余
                        Token token;

                        var basetype = ((非终结符.基础类型.处理(亲) as 类型声明)!.类型定义 as 基础类型描述)!;
                        var record = ((亲 as 类型声明)!.类型定义 as 记录类型描述)!;

                        record.字段.Add(("", basetype));
                        非终结符.字段名表.处理(亲);

                        token = _TokenList[_Index];
                        if (!终结符.分号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        非终结符.域描述余.处理(亲);
                        return null;
                    },
                    [19] = (亲) => {//数组类型 字段名表 ; 域描述余
                        Token token;

                        var arraytype = ((非终结符.数组类型.处理(亲) as 类型声明)!.类型定义 as 数组类型描述)!;

                        var record = ((亲 as 类型声明)!.类型定义 as 记录类型描述)!;
                        record.字段.Add(("", arraytype));
                        非终结符.字段名表.处理(亲);

                        token = _TokenList[_Index];
                        if (!终结符.分号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        非终结符.域描述余.处理(亲);

                        return null;
                    },
                },
                [非终结符.域描述余.Content] = new() {
                    [20] = (亲) => {//
                        return null;
                    },
                    [21] = (亲) => {//域描述表
                        非终结符.域描述表.处理(亲);
                        return null;
                    },
                },
                [非终结符.字段名表.Content] = new() {
                    [22] = (亲) => {//ID 字段名余
                        Token token;
                        var record = ((亲 as 类型声明)!.类型定义 as 记录类型描述)!;

                        token = _TokenList[_Index];
                        if (!终结符.Id("字段名").Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        record.字段.Add((token.Terminal.Content, record.字段.Last().类型));
                        _Index++;

                        非终结符.字段名余.处理(亲);
                        return null;
                    },
                },
                [非终结符.字段名余.Content] = new() {
                    [23] = (亲) => {//
                        return null;
                    },
                    [24] = (亲) => {//, 字段名表
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.逗号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;
                        非终结符.字段名表.处理(亲);
                        return null;
                    },
                },
                [非终结符.变量声明.Content] = new() {
                    [25] = (亲) => {//
                        return null;
                    },
                    [26] = (亲) => {//VAR 变量声表
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.VAR.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        非终结符.变量声表.处理(亲);

                        (亲! as 过程声明)!.变量声明列表.RemoveAll(v => v.变量名 == "");

                        return null;
                    },
                },
                [非终结符.变量声表.Content] = new() {
                    [27] = (亲) => {//类型定义 标量名表 ; 变量声余
                        Token token;

                        var type = (非终结符.类型定义.处理(亲) as 类型声明)!.类型定义;

                        (亲! as 过程声明)!.变量声明列表.Add(new 变量声明(0, 亲!, "", type));
                        非终结符.变量名表.处理(亲);

                        token = _TokenList[_Index];
                        if (!终结符.分号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        非终结符.变量声余.处理(亲);

                        return null;
                    },
                },
                [非终结符.变量声余.Content] = new() {
                    [28] = (亲) => {//
                        return null;
                    },
                    [29] = (亲) => {//变量声表
                        非终结符.变量声表.处理(亲);
                        return null;
                    },
                },
                [非终结符.变量名表.Content] = new() {
                    [30] = (亲) => {//ID 变量名余
                        Token token;
                        var proc = (亲 as 过程声明)!;

                        token = _TokenList[_Index];
                        if (!终结符.Id("变量名").Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        proc.变量声明列表.Add(new 变量声明(token.Line, 亲!, token.Terminal.Content, proc.变量声明列表.Last().变量类型));
                        _Index++;

                        非终结符.变量名余.处理(亲);

                        return null;
                    },
                },
                [非终结符.变量名余.Content] = new() {
                    [31] = (亲) => {//
                        return null;
                    },
                    [32] = (亲) => {//, 变量名表
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.逗号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        非终结符.变量名表.处理(亲);

                        return null;
                    },
                },
                [非终结符.过程声明.Content] = new() {
                    [33] = (亲) => {//
                        return null;
                    },
                    [34] = (亲) => {//PROCEDURE ID ( 参数列表 ) ； 声明部分 过程主体 过程声明
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.PROCEDURE.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var line = token.Line;
                        _Index++;

                        token = _TokenList[_Index];
                        if (!终结符.Id("过程名").Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var proc = new 过程声明(line, 亲, token.Terminal.Content);
                        _Index++;

                        token = _TokenList[_Index];
                        if (!终结符.左圆.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        非终结符.参数列表.处理(proc);

                        token = _TokenList[_Index];
                        if (!终结符.右圆.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        token = _TokenList[_Index];
                        if (!终结符.分号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        非终结符.声明部分.处理(proc);
                        非终结符.过程主体.处理(proc);
                        非终结符.过程声明.处理(proc);

                        (亲 as 过程声明)!.过程声明列表.Add(proc);
                        return null;
                    },
                },
                [非终结符.参数列表.Content] = new() {
                    [35] = (亲) => {//
                        return null;
                    },
                    [36] = (亲) => {//参数描表
                        非终结符.参数描表.处理(亲);
                        (亲 as 过程声明)!.参数列表.RemoveAll(a => a.参数.变量名 == "");
                        return null;
                    },
                },
                [非终结符.参数描表.Content] = new() {
                    [37] = (亲) => {//形式参数 形参更多
                        非终结符.形式参数.处理(亲);
                        非终结符.形参更多.处理(亲);
                        return null;
                    },
                },
                [非终结符.形参更多.Content] = new() {
                    [38] = (亲) => {//
                        return null;
                    },
                    [39] = (亲) => {//; 参数描表
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.分号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;
                        非终结符.参数描表.处理(亲);
                        return null;
                    },
                },
                [非终结符.形式参数.Content] = new() {
                    [40] = (亲) => {//类型定义 参量名表
                        var argtype = (非终结符.类型定义.处理(亲) as 类型声明)!.类型定义;
                        (亲 as 过程声明)!.参数列表.Add((new 变量声明(0, 亲, "", argtype), false));
                        非终结符.参量名表.处理(亲);
                        return null;
                    },
                    [41] = (亲) => {//VAR 类型定义 参量名表
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.VAR.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        var argtype = (非终结符.类型定义.处理(亲) as 类型声明)!.类型定义;
                        (亲 as 过程声明)!.参数列表.Add((new 变量声明(0, 亲, "", argtype), true));

                        非终结符.参量名表.处理(亲);
                        return null;
                    },
                },
                [非终结符.参量名表.Content] = new() {
                    [42] = (亲) => {//ID 参量名余
                        Token token;
                        var proc = (亲 as 过程声明)!;
                        var lastarg = proc.参数列表.Last();

                        token = _TokenList[_Index];
                        if (!终结符.Id("形参名").Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        proc.参数列表.Add((new 变量声明(token.Line, 亲!, token.Terminal.Content, lastarg.参数.变量类型), lastarg.引用));
                        _Index++;

                        非终结符.参量名余.处理(亲);

                        return null;
                    },
                },
                [非终结符.参量名余.Content] = new() {
                    [43] = (亲) => {//
                        return null;
                    },
                    [44] = (亲) => {//, 参量名表
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.逗号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        非终结符.参量名表.处理(亲);

                        return null;
                    },
                },
                [非终结符.过程主体.Content] = new() {
                    [45] = (亲) => {//BEGIN 语句列表 END
                        Token token;

                        var curstmlist = stmlist;
                        stmlist = (亲 as 过程声明)!.过程体;

                        token = _TokenList[_Index];
                        if (!终结符.BEGIN.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        非终结符.语句列表.处理(亲);

                        token = _TokenList[_Index];
                        if (!终结符.END.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        stmlist = curstmlist;

                        return null;
                    },
                },
                [非终结符.语句列表.Content] = new() {
                    [46] = (亲) => {//一条语句 语句更多
                        非终结符.一条语句.处理(亲);
                        非终结符.语句更多.处理(亲);
                        return null;
                    },
                },
                [非终结符.语句更多.Content] = new() {
                    [47] = (亲) => {//
                        return null;
                    },
                    [48] = (亲) => {//; 语句更多
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.分号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        非终结符.语句列表.处理(亲);

                        return null;
                    },
                },
                [非终结符.一条语句.Content] = new() {
                    [49] = (亲) => {//条件语句
                        非终结符.条件语句.处理(亲);
                        return null;
                    },
                    [50] = (亲) => {//循环语句
                        非终结符.循环语句.处理(亲);
                        return null;
                    },
                    [51] = (亲) => {//输入语句
                        非终结符.输入语句.处理(亲);
                        return null;
                    },
                    [52] = (亲) => {//输出语句
                        非终结符.输出语句.处理(亲);
                        return null;
                    },
                    [53] = (亲) => {//返回语句
                        非终结符.返回语句.处理(亲);
                        return null;
                    },
                    [54] = (亲) => {//ID 赋调语句
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.ID.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        stmlist.Add(new 调用语句(token.Line, 亲!, token.Terminal.Content));
                        _Index++;

                        非终结符.赋调语句.处理(亲);

                        return null;
                    },
                },
                [非终结符.赋调语句.Content] = new() {
                    [55] = (亲) => {//赋值语句
                        非终结符.赋值语句.处理(亲);
                        return null;
                    },
                    [56] = (亲) => {//调用语句
                        非终结符.调用语句.处理(亲);
                        return null;
                    },
                },
                [非终结符.赋值语句.Content] = new() {
                    [57] = (亲) => {//变量赘余 := 算术式子
                        Token token;

                        var asstm = new 赋值语句(stmlist.Last().行号, 亲!);

                        _exp = new 表达式构造器(asstm);
                        _exp.Push(new 表达式(stmlist.Last().行号, asstm, '$', (stmlist.Last() as 调用语句)!.过程名));
                        非终结符.变量赘余.处理(asstm);
                        asstm.左 = _exp!.Get();

                        token = _TokenList[_Index];
                        if (!终结符.赋值.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        _exp = new 表达式构造器(asstm);
                        非终结符.算术式子.处理(asstm);
                        asstm.右 = _exp!.Get();

                        stmlist.RemoveAt(stmlist.Count - 1);
                        stmlist.Add(asstm);
                        return null;
                    },
                },
                [非终结符.条件语句.Content] = new() {
                    [58] = (亲) => {//IF 条件式子 THEN 语句列表 ELSE 语句列表 FI
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.IF.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var condstm = new 条件语句(token.Line, 亲!);
                        _Index++;

                        _exp = new 表达式构造器(condstm);
                        非终结符.条件式子.处理(condstm);

                        condstm.条件 = _exp!.Get();
                        var curstmlist = stmlist;
                        token = _TokenList[_Index];

                        if (!终结符.THEN.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        stmlist = condstm.THEN;
                        非终结符.语句列表.处理(condstm);

                        token = _TokenList[_Index];
                        if (!终结符.ELSE.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        stmlist = condstm.ELSE;
                        非终结符.语句列表.处理(condstm);

                        token = _TokenList[_Index];
                        if (!终结符.FI.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        stmlist = curstmlist;
                        stmlist.Add(condstm);

                        return null;
                    },
                },
                [非终结符.循环语句.Content] = new() {
                    [59] = (亲) => {//WHILE 条件式子 DO 语句列表 ENDWH
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.WHILE.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var loopstm = new 循环语句(token.Line, 亲!);
                        _Index++;

                        _exp = new 表达式构造器(loopstm);
                        非终结符.条件式子.处理(loopstm);
                        loopstm.条件 = _exp!.Get();

                        var curstmlist = stmlist;

                        token = _TokenList[_Index];
                        if (!终结符.DO.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        stmlist = loopstm.循环体;
                        非终结符.语句列表.处理(loopstm);

                        token = _TokenList[_Index];
                        if (!终结符.ENDWH.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        stmlist = curstmlist;
                        stmlist.Add(loopstm);

                        return null;
                    },
                },
                [非终结符.输入语句.Content] = new() {
                    [60] = (亲) => {//READ ( ID ) 
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.READ.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var readstm = new 输入语句(token.Line, 亲!);
                        _Index++;

                        token = _TokenList[_Index];
                        if (!终结符.左圆.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        token = _TokenList[_Index];
                        if (!终结符.ID.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        readstm.待输入量 = new 表达式(token.Line, readstm, '$', token.Terminal.Content);
                        _Index++;

                        token = _TokenList[_Index];
                        if (!终结符.右圆.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        stmlist.Add(readstm);

                        return null;
                    },
                },
                [非终结符.输出语句.Content] = new() {
                    [61] = (亲) => {//WRITE ( 算术式子 )
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.WRITE.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var writestm = new 输出语句(token.Line, 亲!);
                        _Index++;

                        token = _TokenList[_Index];
                        if (!终结符.左圆.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        _exp = new 表达式构造器(writestm);
                        非终结符.算术式子.处理(writestm);
                        writestm.待输出值 = _exp!.Get();

                        token = _TokenList[_Index];
                        if (!终结符.右圆.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        stmlist.Add(writestm);

                        return null;
                    },
                },
                [非终结符.返回语句.Content] = new() {
                    [62] = (亲) => {//RETURN
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.RETURN.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var retstm = new 返回语句(token.Line, 亲!);
                        _Index++;

                        stmlist.Add(retstm);

                        return null;
                    },
                },
                [非终结符.调用语句.Content] = new() {
                    [63] = (亲) => {//( 实参列表 )
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.左圆.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        var callstm = stmlist.Last();
                        _Index++;

                        非终结符.实参列表.处理(callstm);

                        token = _TokenList[_Index];
                        if (!终结符.右圆.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;

                        return null;
                    },
                },
                [非终结符.实参列表.Content] = new() {
                    [64] = (亲) => {//
                        return null;
                    },
                    [65] = (亲) => {//算术式子 实参更多
                        _exp = new 表达式构造器((亲 as 调用语句)!);
                        非终结符.算术式子.处理(亲);
                        (亲 as 调用语句)!.实参列表.Add(_exp!.Get());
                        非终结符.实参更多.处理(亲);
                        return null;
                    },
                },
                [非终结符.实参更多.Content] = new() {
                    [66] = (亲) => {//
                        return null;
                    },
                    [67] = (亲) => {//, 实参列表
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.逗号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _Index++;
                        非终结符.实参列表.处理(亲);
                        return null;
                    },
                },
                [非终结符.条件式子.Content] = new() {
                    [68] = (亲) => {//算术式子 比较符号 算术式子
                        非终结符.算术式子.处理(亲);
                        非终结符.比较符号.处理(亲);
                        非终结符.算术式子.处理(亲);
                        return null;
                    },
                },
                [非终结符.算术式子.Content] = new() {
                    [69] = (亲) => {//运算项目 其余项目
                        非终结符.运算项目.处理(亲);
                        非终结符.其余项目.处理(亲);
                        return null;
                    },
                },
                [非终结符.其余项目.Content] = new() {
                    [70] = (亲) => {//
                        return null;
                    },
                    [71] = (亲) => {//加减符号 算术式子
                        非终结符.加减符号.处理(亲);
                        非终结符.算术式子.处理(亲);
                        return null;
                    },
                },
                [非终结符.运算项目.Content] = new() {
                    [72] = (亲) => {//算术因子 其余因子
                        非终结符.算术因子.处理(亲);
                        非终结符.其余因子.处理(亲);
                        return null;
                    },
                },
                [非终结符.其余因子.Content] = new() {
                    [73] = (亲) => {//
                        return null;
                    },
                    [74] = (亲) => {//乘除符号 运算项目
                        非终结符.乘除符号.处理(亲);
                        非终结符.运算项目.处理(亲);
                        return null;
                    },
                },
                [非终结符.算术因子.Content] = new() {
                    [75] = (亲) => {//( 算术式子 )
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.左圆.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(token);
                        _Index++;

                        非终结符.算术式子.处理(亲);

                        token = _TokenList[_Index];
                        if (!终结符.右圆.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(token);
                        _Index++;

                        return null;
                    },
                    [76] = (亲) => {//INTC
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.INTC.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(new 表达式(token.Line, 亲!, '#', token.Terminal.Content));
                        _Index++;
                        return null;
                    },
                    [77] = (亲) => {//ID 变量赘余
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.ID.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(new 表达式(token.Line, 亲!, '$', token.Terminal.Content));
                        _Index++;

                        非终结符.变量赘余.处理(亲);

                        return null;
                    },
                },
                [非终结符.变量赘余.Content] = new() {
                    [78] = (亲) => {//
                        return null;
                    },
                    [79] = (亲) => {//[ 算术式子 ]
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.左方.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        //_exp!.Push(token); 这里特殊处理一下，把 arr[exp]处理成arr_(exp)
                        _exp!.Push(new Token(token.Line, new 终结符(终结符.TypeEnum.SY, "_")));
                        _exp!.Push(new Token(token.Line, 终结符.左圆));
                        _Index++;

                        非终结符.算术式子.处理(亲);

                        token = _TokenList[_Index];
                        if (!终结符.右方.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(new Token(token.Line, 终结符.右圆));
                        _Index++;

                        return null;
                    },
                    [80] = (亲) => {//.记录域名
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.单点.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(token);
                        _Index++;
                        非终结符.记录域名.处理(亲);
                        return null;
                    },
                },
                [非终结符.记录域名.Content] = new() {
                    [81] = (亲) => {//ID 域名更多
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.Id("字段名").Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(new 表达式(token.Line, 亲!, '$', token.Terminal.Content));
                        _Index++;

                        非终结符.域名更多.处理(亲);

                        return null;
                    },
                },
                [非终结符.域名更多.Content] = new() {
                    [82] = (亲) => {//
                        return null;
                    },
                    [83] = (亲) => {//[算术式子]
                        Token token;

                        token = _TokenList[_Index];
                        if (!终结符.左方.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        //_exp!.Push(token); 这里特殊处理一下，把 arr[exp]处理成arr_(exp)
                        _exp!.Push(new Token(token.Line, new 终结符(终结符.TypeEnum.SY, "_")));
                        _exp!.Push(new Token(token.Line, 终结符.左圆));
                        _Index++;

                        非终结符.算术式子.处理(亲);

                        token = _TokenList[_Index];
                        if (!终结符.右方.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(new Token(token.Line, 终结符.右圆));
                        _Index++;

                        return null;
                    },
                },
                [非终结符.比较符号.Content] = new() {
                    [84] = (亲) => {//<
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.小于.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(token);
                        _Index++;
                        return null;
                    },
                    [85] = (亲) => {//=
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.等号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(token);
                        _Index++;
                        return null;
                    },
                },
                [非终结符.加减符号.Content] = new() {
                    [86] = (亲) => {//+
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.加号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(token);
                        _Index++;
                        return null;
                    },
                    [87] = (亲) => {//-
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.减号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(token);
                        _Index++;
                        return null;
                    },
                },
                [非终结符.乘除符号.Content] = new() {
                    [88] = (亲) => {//*
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.乘号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(token);
                        _Index++;
                        return null;
                    },
                    [89] = (亲) => {///
                        Token token;
                        token = _TokenList[_Index];
                        if (!终结符.除号.Matches(token)) {
                            throw new 语法分析异常(token);
                        }
                        _exp!.Push(token);
                        _Index++;
                        return null;
                    },
                }
            };

        public static 语法树 分析_递归下降(List<Token> tokenList) {
            _TokenList = tokenList;
            _Index = 0;
            try {
                var proc = (非终结符.总体程序.处理(null) as 过程声明)!;
                return new 语法树(proc);
            } catch {
                throw new 语法分析异常(_TokenList[_Index]);
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
                                    curType = new 自定类型描述(token.Line, token.Terminal.Content) { 行号 = curLine };
                                }
                                break;
                            case 10://基础类型→INTEFER
                                if (false) {
                                } else if (terminal.Is(终结符.INTEGER)) {
                                    curType = isArray ?
                                        new 数组类型描述(curLine, arrayLow, arrayTop, 基础类型描述.整数类型(token.Line)) :
                                        基础类型描述.整数类型(token.Line);
                                    isArray = false;
                                }
                                break;
                            case 11://基础类型→CHAR
                                if (false) {
                                } else if (terminal.Is(终结符.CHAR)) {
                                    curType = isArray ?
                                        new 数组类型描述(curLine, arrayLow, arrayTop, 基础类型描述.字符类型(token.Line)) :
                                        基础类型描述.字符类型(token.Line);
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
                                    curType = new 记录类型描述(curLine) {
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
