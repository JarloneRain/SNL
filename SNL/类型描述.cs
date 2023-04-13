using System.Collections.Generic;

namespace SNL {
    internal abstract class 类型描述 {
        public virtual 类型描述 获取原型(符号表? 局部符号表 = null) => this;
        public virtual bool 能接受(类型描述 that) => $"{this}" == $"{that}";
        public virtual bool 是整数(符号表? 局部符号表 = null) => false;
        public virtual bool 是字符(符号表? 局部符号表 = null) => false;
        public virtual bool 是布尔(符号表? 局部符号表 = null) => false;
        public virtual bool 是数组(符号表? 局部符号表 = null) => false;
        public virtual bool 是记录(符号表? 局部符号表 = null) => false;
        public virtual List<语义错误> 语义检查() => new();
        public enum 类型类别Enum {
            未知类型,
            基础类型,
            数组类型,
            记录类型,
            自定类型
        }
        public 类型类别Enum 类型类别 { get; init; } = 类型类别Enum.未知类型;
        public int 行号 { get; init; } = 0;
        public static 未知类型描述 未知类型 => new 未知类型描述();
    }
    internal class 未知类型描述 : 类型描述 {
        public override bool 能接受(类型描述 that) => false;
    }
    internal class 基础类型描述 : 类型描述 {
        public override bool 是整数(符号表? 局部符号表 = null) => 类型 == 基础类型Enum.整数类型;
        public override bool 是字符(符号表? 局部符号表 = null) => 类型 == 基础类型Enum.字符类型;
        public override bool 是布尔(符号表? 局部符号表 = null) => 类型 == 基础类型Enum.布尔类型;
        public enum 基础类型Enum {
            整数类型,
            字符类型,
            布尔类型,
        }
        public 基础类型Enum 类型 { get; init; } = 基础类型Enum.整数类型;
        public override string ToString() => 类型.ToString();
        public static 基础类型描述 整数类型(int 行号 = 0) => new 基础类型描述 {
            行号 = 行号,
            类型类别 = 类型类别Enum.基础类型,
            类型 = 基础类型Enum.整数类型,
        };
        public static 基础类型描述 字符类型(int 行号 = 0) => new 基础类型描述 {
            行号 = 行号,
            类型类别 = 类型类别Enum.基础类型,
            类型 = 基础类型Enum.字符类型,
        };
        public static 基础类型描述 布尔类型(int 行号 = 0) => new 基础类型描述 {
            行号 = 行号,
            类型类别 = 类型类别Enum.基础类型,
            类型 = 基础类型Enum.布尔类型,
        };
    }
    internal class 记录类型描述 : 类型描述 {
        public override bool 是记录(符号表? 局部符号表 = null) => true;
        public List<(string 名称, 类型描述 类型)> 字段 { get; init; } = new();
        public override string ToString() {
            string s = "RECORD";
            foreach (var t in 字段) {
                s += $"\n\t{t.名称}\t{t.类型}";
            }
            return s;
        }
        public override List<语义错误> 语义检查() {
            List<语义错误> 语义错误列表 = new();
            字段.ForEach(f => {
                if (f.类型.类型类别 == 类型类别Enum.数组类型) {
                    语义错误列表.Add((f.类型 as 数组类型描述)!.语义检查()[0]);
                }
                if (字段.FindAll(rf => rf.名称 == f.名称).Count > 1) {
                    语义错误列表.Add(new 语义错误 {
                        行号 = 行号,
                        错误类别 = 语义错误.语义错误类别Enum.重复的标识符,
                    });
                }
            });
            return 语义错误列表;
        }
    }
    internal class 数组类型描述 : 类型描述 {
        public override bool 是数组(符号表? 局部符号表 = null) => true;
        public int 下界 { get; init; }
        public int 上界 { get; init; }
        public 基础类型描述 元素类型 { get; init; } = 基础类型描述.整数类型();
        public override List<语义错误> 语义检查() => new() { 下界 <= 上界 ? 语义错误.OK : new 语义错误 {
            行号 = 行号,
            错误类别 =语义错误.  语义错误类别Enum.数组下标非法
        }};
        public override string ToString() => $"ARRAY[{下界}..{上界}]{元素类型}";
    }
    internal class 自定类型描述 : 类型描述 {
        public override 类型描述 获取原型(符号表? 局部符号表 = null) {
            if (局部符号表 == null) {
                return 未知类型;
            }
            var fs = 局部符号表![类型名];
            if (fs.Count != 1) {
                return 未知类型;
            }
            var f = fs[0];
            if (f.表项类别 != 符号表.符号表项.表项类别Enum.类型) {
                return 未知类型;
            }
            return (f as 符号表.类型)!.描述;
        }
        public override bool 是整数(符号表? 局部符号表 = null) => 获取原型(局部符号表).是整数(局部符号表);
        public override bool 是字符(符号表? 局部符号表 = null) => 获取原型(局部符号表).是字符(局部符号表);
        public override bool 是记录(符号表? 局部符号表 = null) => 获取原型(局部符号表).是记录(局部符号表);
        public override bool 是数组(符号表? 局部符号表 = null) => 获取原型(局部符号表).是数组(局部符号表);
        public string 类型名 { get; init; }
        public 自定类型描述(string 类型名) {
            this.类型类别 = 类型类别Enum.自定类型;
            this.类型名 = 类型名;
        }
        public override string ToString() => $"{类型名}:{获取原型()}";
    }
}
