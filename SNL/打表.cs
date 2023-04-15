using System.Collections.Generic;

namespace SNL {
    //关键字和符号的终结符对象
    internal partial class 终结符 : 结符 {
        public readonly static 终结符 ID = new(TypeEnum.ID, "");
        public readonly static 终结符 INTC = new(TypeEnum.NM, "");
        /********************************************************************/
        public readonly static 终结符 BEGIN = new(TypeEnum.KW, "BEGIN");
        public readonly static 终结符 END = new(TypeEnum.KW, "END");
        public readonly static 终结符 PROGRAM = new(TypeEnum.KW, "PROGRAM");
        public readonly static 终结符 VAR = new(TypeEnum.KW, "VAR");
        public readonly static 终结符 TYPE = new(TypeEnum.KW, "TYPE");
        public readonly static 终结符 PROCEDURE = new(TypeEnum.KW, "PROCEDURE");
        public readonly static 终结符 WHILE = new(TypeEnum.KW, "WHILE");
        public readonly static 终结符 ENDWH = new(TypeEnum.KW, "ENDWH");
        public readonly static 终结符 INTEGER = new(TypeEnum.KW, "INTEGER");
        public readonly static 终结符 CHAR = new(TypeEnum.KW, "CHAR");
        public readonly static 终结符 ARRAY = new(TypeEnum.KW, "ARRAY");
        public readonly static 终结符 OF = new(TypeEnum.KW, "OF");
        public readonly static 终结符 RECORD = new(TypeEnum.KW, "RECORD");
        public readonly static 终结符 IF = new(TypeEnum.KW, "IF");
        public readonly static 终结符 THEN = new(TypeEnum.KW, "THEN");
        public readonly static 终结符 ELSE = new(TypeEnum.KW, "ELSE");
        public readonly static 终结符 FI = new(TypeEnum.KW, "FI");
        public readonly static 终结符 DO = new(TypeEnum.KW, "DO");
        public readonly static 终结符 WRITE = new(TypeEnum.KW, "WRITE");
        public readonly static 终结符 READ = new(TypeEnum.KW, "READ");
        public readonly static 终结符 RETURN = new(TypeEnum.KW, "RETURN");
        /********************************************************************/
        public readonly static 终结符 加号 = new(TypeEnum.SY, "+");
        public readonly static 终结符 减号 = new(TypeEnum.SY, "-");
        public readonly static 终结符 乘号 = new(TypeEnum.SY, "*");
        public readonly static 终结符 除号 = new(TypeEnum.SY, "/");
        public readonly static 终结符 小于 = new(TypeEnum.SY, "<");
        public readonly static 终结符 左圆 = new(TypeEnum.SY, "(");
        public readonly static 终结符 右圆 = new(TypeEnum.SY, ")");
        public readonly static 终结符 左方 = new(TypeEnum.SY, "[");
        public readonly static 终结符 右方 = new(TypeEnum.SY, "]");
        public readonly static 终结符 单点 = new(TypeEnum.SY, ".");
        public readonly static 终结符 分号 = new(TypeEnum.SY, ";");
        public readonly static 终结符 左花 = new(TypeEnum.SY, "{");
        public readonly static 终结符 右花 = new(TypeEnum.SY, "}");
        public readonly static 终结符 空格 = new(TypeEnum.SY, "BLANK");
        public readonly static 终结符 单引 = new(TypeEnum.SY, "\'");
        public readonly static 终结符 等号 = new(TypeEnum.SY, "=");
        public readonly static 终结符 双点 = new(TypeEnum.SY, "..");
        public readonly static 终结符 赋值 = new(TypeEnum.SY, ":=");
        public readonly static 终结符 逗号 = new(TypeEnum.SY, ",");
        public readonly static 终结符 EOF = new(TypeEnum.SY, "~");
    }
    //全部的非终结符的对象
    internal partial class 非终结符 : 结符 {
        public static 非终结符 总体程序 = new("总体程序");
        public static 非终结符 声明部分 = new("声明部分");
        public static 非终结符 过程主体 = new("过程主体");
        public static 非终结符 类型声明 = new("类型声明");
        public static 非终结符 变量声明 = new("变量声明");
        public static 非终结符 过程声明 = new("过程声明");
        public static 非终结符 类型声表 = new("类型声表");
        public static 非终结符 类型定义 = new("类型定义");
        public static 非终结符 类型声余 = new("类型声余");
        public static 非终结符 基础类型 = new("基础类型");
        public static 非终结符 结构类型 = new("结构类型");
        public static 非终结符 数组类型 = new("数组类型");
        public static 非终结符 记录类型 = new("记录类型");
        public static 非终结符 数组下界 = new("数组下界");
        public static 非终结符 数组上界 = new("数组上界");
        public static 非终结符 域描述表 = new("域描述表");
        public static 非终结符 字段名表 = new("字段名表");
        public static 非终结符 域描述余 = new("域描述余");
        public static 非终结符 字段名余 = new("字段名余");
        public static 非终结符 变量声表 = new("变量声表");
        public static 非终结符 变量名表 = new("变量名表");
        public static 非终结符 变量声余 = new("变量声余");
        public static 非终结符 变量名余 = new("变量名余");
        public static 非终结符 参数列表 = new("参数列表");
        public static 非终结符 参数描表 = new("参数描表");
        public static 非终结符 形式参数 = new("形式参数");
        public static 非终结符 形参更多 = new("形参更多");
        public static 非终结符 参量名表 = new("参量名表");
        public static 非终结符 参量名余 = new("参量名余");
        public static 非终结符 语句列表 = new("语句列表");
        public static 非终结符 一条语句 = new("一条语句");
        public static 非终结符 语句更多 = new("语句更多");
        public static 非终结符 条件语句 = new("条件语句");
        public static 非终结符 循环语句 = new("循环语句");
        public static 非终结符 输入语句 = new("输入语句");
        public static 非终结符 输出语句 = new("输出语句");
        public static 非终结符 返回语句 = new("返回语句");
        public static 非终结符 赋调语句 = new("赋调语句");
        public static 非终结符 赋值语句 = new("赋值语句");
        public static 非终结符 调用语句 = new("调用语句");
        public static 非终结符 变量赘余 = new("变量赘余");
        public static 非终结符 算术式子 = new("算术式子");
        public static 非终结符 条件式子 = new("条件式子");
        public static 非终结符 实参列表 = new("实参列表");
        public static 非终结符 实参更多 = new("实参更多");
        public static 非终结符 比较符号 = new("比较符号");
        public static 非终结符 运算项目 = new("运算项目");
        public static 非终结符 其余项目 = new("其余项目");
        public static 非终结符 加减符号 = new("加减符号");
        public static 非终结符 算术因子 = new("算术因子");
        public static 非终结符 其余因子 = new("其余因子");
        public static 非终结符 乘除符号 = new("乘除符号");
        public static 非终结符 记录域名 = new("记录域名");
        public static 非终结符 域名更多 = new("域名更多");
    }
    //全部的产生式
    internal partial class 非终结符 : 结符 {
        static readonly 结符[] 空产生式 = new 结符[0];
        static readonly 结符[][] 产生式 = new 结符[][]{
            //总体程序
            new 结符[]{终结符.PROGRAM,终结符.Id("程序名"),声明部分,过程主体},
            //声明部分
            new 结符[]{类型声明,变量声明,过程声明},
            //类型声明
            空产生式,
            new 结符[]{终结符.TYPE,类型声表},
            //类型声表
            new 结符[]{终结符.Id("类型名"),终结符.等号,类型定义,终结符.分号,类型声余},
            //类型声余
            空产生式,
            new 结符[]{类型声表},
            //类型定义
            new 结符[]{基础类型},
            new 结符[]{结构类型},
            new 结符[]{终结符.Id("类型名")},
            //基础类型
            new 结符[]{终结符.INTEGER},
            new 结符[]{终结符.CHAR},
            //结构类型
            new 结符[]{数组类型},
            new 结符[]{记录类型},
            //数组类型
            new 结符[]{终结符.ARRAY,终结符.左方,数组下界,终结符.双点,数组上界,终结符.右方,终结符.OF,基础类型},
            //数组下界
            new 结符[]{终结符.INTC},
            //数组上界
            new 结符[]{终结符.INTC},
            //记录类型
            new 结符[]{终结符.RECORD,域描述表,终结符.END},
            //域描述表
            new 结符[]{基础类型,字段名表,终结符.分号,域描述余},
            new 结符[]{数组类型,字段名表,终结符.分号,域描述余},
            //域描述余
            空产生式,
            new 结符[]{域描述表},
            //字段名表
            new 结符[]{终结符.Id("字段名"),字段名余},
            //字段名余
            空产生式,
            new 结符[]{终结符.逗号,字段名表},
            //变量声明
            空产生式,
            new 结符[]{终结符.VAR,变量声表},
            //变量声表
            new 结符[]{类型定义,变量名表,终结符.分号,变量声余},
            //变量声余
            空产生式,
            new 结符[]{变量声表},
            //变量名表
            new 结符[]{终结符.Id("变量名"),变量名余},
            //变量名余
            空产生式,
            new 结符[]{终结符.逗号,变量名表},
            //过程声明
            空产生式,
            new 结符[]{终结符.PROCEDURE,终结符.Id("过程名"),终结符.左圆,参数列表,终结符.右圆,终结符.分号,声明部分,过程主体,过程声明},
            //参数列表
            空产生式,
            new 结符[]{参数描表},
            //参数描表
            new 结符[]{形式参数,形参更多},
            //形参更多
            空产生式,
            new 结符[]{终结符.分号,参数描表},
            //形式参数
            new 结符[]{类型定义,参量名表},
            new 结符[]{终结符.VAR,类型定义,参量名表},
            //参量名表
            new 结符[]{终结符.Id("形参名"),参量名余},
            //参量名余
            空产生式,
            new 结符[]{终结符.逗号,参量名表},
            //过程主体
            new 结符[]{终结符.BEGIN,语句列表,终结符.END},
            //语句列表
            new 结符[]{一条语句,语句更多},
            //语句更多
            空产生式,
            new 结符[]{终结符.分号,语句列表},
            //一条语句
            new 结符[]{条件语句},
            new 结符[]{循环语句},
            new 结符[]{输入语句},
            new 结符[]{输出语句},
            new 结符[]{返回语句},
            new 结符[]{终结符.Id("赋调名"),赋调语句},
            //赋调语句
            new 结符[]{赋值语句},
            new 结符[]{调用语句},
            //赋值语句
            new 结符[]{变量赘余,终结符.赋值,算术式子},
            //条件语句
            new 结符[]{终结符.IF,条件式子,终结符.THEN,语句列表,终结符.ELSE,语句列表,终结符.FI},
            //循环语句
            new 结符[]{终结符.WHILE,条件式子,终结符.DO,语句列表,终结符.ENDWH},
            //输入语句
            new 结符[]{终结符.READ,终结符.左圆,终结符.Id("输入变量"),终结符.右圆},
            //输出语句
            new 结符[]{终结符.WRITE,终结符.左圆,算术式子,终结符.右圆},
            //返回语句
            new 结符[]{终结符.RETURN},
            //调用语句
            new 结符[]{终结符.左圆,实参列表,终结符.右圆},
            //实参列表
            空产生式,
            new 结符[]{算术式子,实参更多},
            //实参更多
            空产生式,
            new 结符[]{终结符.逗号,实参列表},
            //条件式子
            new 结符[]{算术式子,比较符号,算术式子},
            //算术式子
            new 结符[]{运算项目,其余项目},
            //其余项目
            空产生式,
            new 结符[]{加减符号,算术式子},
            //运算项目
            new 结符[]{算术因子,其余因子},
            //其余因子
            空产生式,
            new 结符[]{乘除符号,运算项目},
            //算术因子
            new 结符[]{终结符.左圆,算术式子,终结符.右圆},
            new 结符[]{终结符.INTC},
            new 结符[]{终结符.Id("变量名"),变量赘余},
            //变量赘余
            空产生式,
            new 结符[]{终结符.左方,算术式子,终结符.右方},
            new 结符[]{终结符.单点,记录域名},
            //记录域名
            new 结符[]{终结符.Id("字段名"),域名更多},
            //域名更多
            空产生式,
            new 结符[]{终结符.左方,算术式子,终结符.右方},
            //比较符号
            new 结符[]{终结符.小于},
            new 结符[]{终结符.等号},
            //加减符号
            new 结符[]{终结符.加号},
            new 结符[]{终结符.减号},
            //乘除符号
            new 结符[]{终结符.乘号},
            new 结符[]{终结符.除号}
        };
    }
    //Predict集，Predict[非终结符.Content][终结符.KeyStr]
    internal partial class 非终结符 : 结符 {
        public static Dictionary<
            string,//终极符的Content
            Dictionary<
                string,//非终极符所接受的终结符的ToKey
                int//产生式的编号
            >
        > Predict = new() {
            [总体程序.Content] = new() {
                [终结符.PROGRAM.KeyStr] = 0,
            },
            [声明部分.Content] = new() {
                [终结符.TYPE.KeyStr] = 1,
                [终结符.VAR.KeyStr] = 1,
                [终结符.PROGRAM.KeyStr] = 1,
                [终结符.BEGIN.KeyStr] = 1,
            },
            [类型声明.Content] = new() {
                [终结符.VAR.KeyStr] = 2,
                [终结符.PROGRAM.KeyStr] = 2,
                [终结符.BEGIN.KeyStr] = 2,
                [终结符.TYPE.KeyStr] = 3,
            },
            [类型声表.Content] = new() {
                [终结符.ID.KeyStr] = 4,
            },
            [类型声余.Content] = new() {
                [终结符.VAR.KeyStr] = 5,
                [终结符.PROCEDURE.KeyStr] = 5,
                [终结符.BEGIN.KeyStr] = 5,
                [终结符.ID.KeyStr] = 6,
            },
            [类型定义.Content] = new() {
                [终结符.INTEGER.KeyStr] = 7,
                [终结符.CHAR.KeyStr] = 7,
                [终结符.ARRAY.KeyStr] = 8,
                [终结符.RECORD.KeyStr] = 8,
                [终结符.ID.KeyStr] = 9,
            },
            [基础类型.Content] = new() {
                [终结符.INTEGER.KeyStr] = 10,
                [终结符.CHAR.KeyStr] = 11,
            },
            [结构类型.Content] = new() {
                [终结符.ARRAY.KeyStr] = 12,
                [终结符.RECORD.KeyStr] = 13,
            },
            [数组类型.Content] = new() {
                [终结符.ARRAY.KeyStr] = 14,
            },
            [数组下界.Content] = new() {
                [终结符.INTC.KeyStr] = 15,
            },
            [数组上界.Content] = new() {
                [终结符.INTC.KeyStr] = 16,
            },
            [记录类型.Content] = new() {
                [终结符.RECORD.KeyStr] = 17,
            },
            [域描述表.Content] = new() {
                [终结符.INTEGER.KeyStr] = 18,
                [终结符.CHAR.KeyStr] = 18,
                [终结符.ARRAY.KeyStr] = 19,
            },
            [域描述余.Content] = new() {
                [终结符.END.KeyStr] = 20,
                [终结符.INTEGER.KeyStr] = 21,
                [终结符.CHAR.KeyStr] = 21,
                [终结符.ARRAY.KeyStr] = 21,
            },
            [字段名表.Content] = new() {
                [终结符.ID.KeyStr] = 22,
            },
            [字段名余.Content] = new() {
                [终结符.分号.KeyStr] = 23,
                [终结符.逗号.KeyStr] = 24,
            },
            [变量声明.Content] = new() {
                [终结符.PROCEDURE.KeyStr] = 25,
                [终结符.BEGIN.KeyStr] = 25,
                [终结符.VAR.KeyStr] = 26,
            },
            [变量声表.Content] = new() {
                [终结符.INTEGER.KeyStr] = 27,
                [终结符.CHAR.KeyStr] = 27,
                [终结符.ARRAY.KeyStr] = 27,
                [终结符.RECORD.KeyStr] = 27,
                [终结符.ID.KeyStr] = 27,
            },
            [变量声余.Content] = new() {
                [终结符.PROCEDURE.KeyStr] = 28,
                [终结符.BEGIN.KeyStr] = 28,
                [终结符.INTEGER.KeyStr] = 29,
                [终结符.CHAR.KeyStr] = 29,
                [终结符.ARRAY.KeyStr] = 29,
                [终结符.RECORD.KeyStr] = 29,
                [终结符.ID.KeyStr] = 29,
            },
            [变量名表.Content] = new() {
                [终结符.ID.KeyStr] = 30,
            },
            [变量名余.Content] = new() {
                [终结符.分号.KeyStr] = 31,
                [终结符.逗号.KeyStr] = 32,
            },
            [过程声明.Content] = new() {
                [终结符.BEGIN.KeyStr] = 33,
                [终结符.PROCEDURE.KeyStr] = 34,
            },
            [参数列表.Content] = new() {
                [终结符.右圆.KeyStr] = 35,
                [终结符.INTEGER.KeyStr] = 36,
                [终结符.CHAR.KeyStr] = 36,
                [终结符.ARRAY.KeyStr] = 36,
                [终结符.RECORD.KeyStr] = 36,
                [终结符.ID.KeyStr] = 36,
                [终结符.VAR.KeyStr] = 36,
            },
            [参数描表.Content] = new() {
                [终结符.INTEGER.KeyStr] = 37,
                [终结符.CHAR.KeyStr] = 37,
                [终结符.ARRAY.KeyStr] = 37,
                [终结符.RECORD.KeyStr] = 37,
                [终结符.ID.KeyStr] = 37,
                [终结符.VAR.KeyStr] = 37,
            },
            [形参更多.Content] = new() {
                [终结符.右圆.KeyStr] = 38,
                [终结符.分号.KeyStr] = 39,
            },
            [形式参数.Content] = new() {
                [终结符.INTEGER.KeyStr] = 40,
                [终结符.CHAR.KeyStr] = 40,
                [终结符.ARRAY.KeyStr] = 40,
                [终结符.RECORD.KeyStr] = 40,
                [终结符.ID.KeyStr] = 40,
                [终结符.VAR.KeyStr] = 41,
            },
            [参量名表.Content] = new() {
                [终结符.ID.KeyStr] = 42,
            },
            [参量名余.Content] = new() {
                [终结符.右圆.KeyStr] = 43,
                [终结符.分号.KeyStr] = 43,
                [终结符.逗号.KeyStr] = 44,
            },
            [过程主体.Content] = new() {
                [终结符.BEGIN.KeyStr] = 45,
            },
            [语句列表.Content] = new() {
                [终结符.ID.KeyStr] = 46,
                [终结符.IF.KeyStr] = 46,
                [终结符.WHILE.KeyStr] = 46,
                [终结符.RETURN.KeyStr] = 46,
                [终结符.READ.KeyStr] = 46,
                [终结符.WRITE.KeyStr] = 46,
            },
            [语句更多.Content] = new() {
                [终结符.ELSE.KeyStr] = 47,
                [终结符.FI.KeyStr] = 47,
                [终结符.END.KeyStr] = 47,
                [终结符.ENDWH.KeyStr] = 47,
                [终结符.分号.KeyStr] = 48,
            },
            [一条语句.Content] = new() {
                [终结符.IF.KeyStr] = 49,
                [终结符.WHILE.KeyStr] = 50,
                [终结符.READ.KeyStr] = 51,
                [终结符.WRITE.KeyStr] = 52,
                [终结符.RETURN.KeyStr] = 53,
                [终结符.ID.KeyStr] = 54
            },
            [赋调语句.Content] = new() {
                [终结符.单点.KeyStr] = 55,
                [终结符.赋值.KeyStr] = 55,
                [终结符.左方.KeyStr] = 55,
                [终结符.左圆.KeyStr] = 56,
            },
            [赋值语句.Content] = new() {
                [终结符.左方.KeyStr] = 57,
                [终结符.单点.KeyStr] = 57,
                [终结符.赋值.KeyStr] = 57,
            },
            [条件语句.Content] = new() {
                [终结符.IF.KeyStr] = 58,
            },
            [循环语句.Content] = new() {
                [终结符.WHILE.KeyStr] = 59,
            },
            [输入语句.Content] = new() {
                [终结符.READ.KeyStr] = 60,
            },
            [输出语句.Content] = new() {
                [终结符.WRITE.KeyStr] = 61,
            },
            [返回语句.Content] = new() {
                [终结符.RETURN.KeyStr] = 62,
            },
            [调用语句.Content] = new() {
                [终结符.左圆.KeyStr] = 63,
            },
            [实参列表.Content] = new() {
                [终结符.右圆.KeyStr] = 64,
                [终结符.ID.KeyStr] = 65,
                [终结符.左圆.KeyStr] = 65,
                [终结符.INTC.KeyStr] = 65,
            },
            [实参更多.Content] = new() {
                [终结符.右圆.KeyStr] = 66,
                [终结符.逗号.KeyStr] = 67,
            },
            [条件式子.Content] = new() {
                [终结符.ID.KeyStr] = 68,
                [终结符.左圆.KeyStr] = 68,
                [终结符.INTC.KeyStr] = 68,
            },
            [算术式子.Content] = new() {
                [终结符.ID.KeyStr] = 69,
                [终结符.左圆.KeyStr] = 69,
                [终结符.INTC.KeyStr] = 69,
            },
            [其余项目.Content] = new() {
                [终结符.小于.KeyStr] = 70,
                [终结符.等号.KeyStr] = 70,
                [终结符.右方.KeyStr] = 70,
                [终结符.THEN.KeyStr] = 70,
                [终结符.ELSE.KeyStr] = 70,
                [终结符.FI.KeyStr] = 70,
                [终结符.DO.KeyStr] = 70,
                [终结符.ENDWH.KeyStr] = 70,
                [终结符.右圆.KeyStr] = 70,
                [终结符.END.KeyStr] = 70,
                [终结符.分号.KeyStr] = 70,
                [终结符.逗号.KeyStr] = 70,
                [终结符.加号.KeyStr] = 71,
                [终结符.减号.KeyStr] = 71,
            },
            [运算项目.Content] = new() {
                [终结符.左圆.KeyStr] = 72,
                [终结符.INTC.KeyStr] = 72,
                [终结符.ID.KeyStr] = 72,
            },
            [其余因子.Content] = new() {
                [终结符.加号.KeyStr] = 73,
                [终结符.减号.KeyStr] = 73,
                [终结符.小于.KeyStr] = 73,
                [终结符.等号.KeyStr] = 73,
                [终结符.右方.KeyStr] = 73,
                [终结符.THEN.KeyStr] = 73,
                [终结符.ELSE.KeyStr] = 73,
                [终结符.FI.KeyStr] = 73,
                [终结符.DO.KeyStr] = 73,
                [终结符.ENDWH.KeyStr] = 73,
                [终结符.右圆.KeyStr] = 73,
                [终结符.END.KeyStr] = 73,
                [终结符.分号.KeyStr] = 73,
                [终结符.逗号.KeyStr] = 73,
                [终结符.乘号.KeyStr] = 74,
                [终结符.除号.KeyStr] = 74,
            },
            [算术因子.Content] = new() {
                [终结符.左圆.KeyStr] = 75,
                [终结符.INTC.KeyStr] = 76,
                [终结符.ID.KeyStr] = 77,
            },
            [变量赘余.Content] = new() {
                [终结符.赋值.KeyStr] = 78,
                [终结符.乘号.KeyStr] = 78,
                [终结符.除号.KeyStr] = 78,
                [终结符.加号.KeyStr] = 78,
                [终结符.减号.KeyStr] = 78,
                [终结符.小于.KeyStr] = 78,
                [终结符.等号.KeyStr] = 78,
                [终结符.右方.KeyStr] = 78,
                [终结符.THEN.KeyStr] = 78,
                [终结符.ELSE.KeyStr] = 78,
                [终结符.FI.KeyStr] = 78,
                [终结符.DO.KeyStr] = 78,
                [终结符.ENDWH.KeyStr] = 78,
                [终结符.右圆.KeyStr] = 78,
                [终结符.END.KeyStr] = 78,
                [终结符.分号.KeyStr] = 78,
                [终结符.逗号.KeyStr] = 78,
                [终结符.左方.KeyStr] = 79,
                [终结符.单点.KeyStr] = 80,
            },
            [记录域名.Content] = new() {
                [终结符.ID.KeyStr] = 81,
            },
            [域名更多.Content] = new() {
                [终结符.赋值.KeyStr] = 82,
                [终结符.乘号.KeyStr] = 82,
                [终结符.除号.KeyStr] = 82,
                [终结符.加号.KeyStr] = 82,
                [终结符.减号.KeyStr] = 82,
                [终结符.小于.KeyStr] = 82,
                [终结符.等号.KeyStr] = 82,
                [终结符.THEN.KeyStr] = 82,
                [终结符.ELSE.KeyStr] = 82,
                [终结符.FI.KeyStr] = 82,
                [终结符.DO.KeyStr] = 82,
                [终结符.ENDWH.KeyStr] = 82,
                [终结符.右圆.KeyStr] = 82,
                [终结符.END.KeyStr] = 82,
                [终结符.分号.KeyStr] = 82,
                [终结符.逗号.KeyStr] = 82,
                [终结符.左方.KeyStr] = 83,
            },
            [比较符号.Content] = new() {
                [终结符.小于.KeyStr] = 84,
                [终结符.等号.KeyStr] = 85,
            },
            [加减符号.Content] = new() {
                [终结符.加号.KeyStr] = 86,
                [终结符.减号.KeyStr] = 87,
            },
            [乘除符号.Content] = new() {
                [终结符.乘号.KeyStr] = 88,
                [终结符.除号.KeyStr] = 89,
            },
        };
    }
}
