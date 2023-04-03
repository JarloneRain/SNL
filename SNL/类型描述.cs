using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNL {
    internal abstract class 类型描述 {
        public enum 类型类别Enum {
            基础类型,
            数组类型,
            记录类型,
            自定类型
        }
        public 类型类别Enum 类型类别 {
            get; init;
        } = 类型类别Enum.自定类型;
        public int 行号 { get; init; }
        public static 基础类型描述 整数类型(int 行号=0) {
            return new 基础类型描述 {
                行号 = 行号,
                类型类别 = 类型类别Enum.基础类型,
                类型 = 基础类型描述.基础类型Enum.整数类型,
            };
        }
        public static 基础类型描述 字符类型(int 行号 = 0) {
            return new 基础类型描述 {
                行号 = 行号,
                类型类别 = 类型类别Enum.基础类型,
                类型 = 基础类型描述.基础类型Enum.字符类型,
            };
        }
        public static 数组类型描述 数组类型(
            int 下界, int 上界,
            类型描述 元素类型,
            int 行号=0
        ) {
            return new 数组类型描述 {
                下界 = 下界,
                上界 = 上界,
                元素类型 = 元素类型,
                行号=行号,
            };
        }
        public static 自定类型描述 自定类型(
            string 自定类型名,
            int 行号 = 0
        ) {
            return new 自定类型描述(自定类型名) {
                行号 = 行号
            };
        }
    }
    internal class 基础类型描述 : 类型描述 {
        public enum 基础类型Enum {
            整数类型,
            字符类型
        }
        public 基础类型Enum 类型 {
            get; init;
        } = 基础类型Enum.整数类型;
        public override string ToString() {
            return 类型.ToString();
        }
    }
    internal class 记录类型描述 : 类型描述 {
        public List<(
            string 名称,
            类型描述 类型
        )> 字段 {
            get; init;
        } = new();
        public override string ToString() {
            string s = "RECORD";
            foreach(var t in 字段) {
                s += $"\n\t{t.名称}{t.类型}";
            }
            return s;
        }
    }
    internal class 数组类型描述 : 类型描述 {
        public int 下界 { get; init; }
        public int 上界 { get; init; }
        public 类型描述? 元素类型 { get; init; }
        public override string ToString() {
            return $"ARRAY[{下界}..{上界}]{元素类型}";
        }
    }
    internal class 自定类型描述 : 类型描述 {
        public string 类型名 { get; init; } 
        public 自定类型描述(string 类型名) {
            this.类型类别 = 类型类别Enum.自定类型;
            this.类型名 = 类型名;
        }
        public override string ToString() {
            return 类型名;
        }
    }
}
