using System.Collections.Generic;

namespace SNL {
    internal class 语义错误 {
        public static 语义错误 OK {
            get;
        } = new 语义错误 {
            错误内容 = 语义错误Enum.OK,
        };
        public 语义错误Enum 错误内容 { get; init; }
        public int 行号 { get; init; } = 0;
    }
    internal enum 语义错误Enum {
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
    internal static class 语义 {
        public static List<语义错误> 分析(语法树 语法树) {
            return 语法树.语义检查();
        }
    }
}
