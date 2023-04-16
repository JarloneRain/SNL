using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SNL {
    internal class 语义错误 {
        internal enum 语义错误类别Enum {
            OK,
            重复的标识符,
            未声明标识符,
            未知的类型,
            非期望的标识符类别,
            非数组的标识符,
            数组下标越界,
            数组下标非法,
            未知的字段,
            赋值语句的左右两边类型不相容,
            向非变量的标识符赋值,
            参数类型错误,
            参数数目错误,
            非过程的标识符,
            非布尔的条件表达式,
            运算符的分量类型不相容,
            未知错误
        }
        public static 语义错误 OK {
            get;
        } = new 语义错误 {
            错误类别 = 语义错误类别Enum.OK,
        };
        public 语义错误类别Enum 错误类别 { get; init; }
        public string 附加说明 { get; set; } = "";
        public int 行号 { get; init; } = 0;
        public override string ToString() => $"{行号}:{错误类别}\n{Regex.Replace(附加说明,@"\s+"," ") }";
    }

    internal static class 语义 {
        public static List<语义错误> 分析(语法树 语法树) {
            语法树.更新符号表();
            var 语义错误列表 = 语法树.语义检查();
            语义错误列表.RemoveAll(e => e.错误类别 == 语义错误.语义错误类别Enum.OK);
            return 语义错误列表;
        }
    }
}
