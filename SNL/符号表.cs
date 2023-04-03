using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNL {
    using static SNL.语法树;
    using 参数列表类型 = List<(类型描述 类型, bool 引用)>;
    internal partial class 符号表 {
        符号表? 外表 = null;
        public List<符号表项> 表项内容 {
            get;
        } = new();
        public 符号表(符号表? 外表 = null) {
            this.外表 = 外表;
        }
        public List<符号表项> this[string 符号名] {
            get {
                var find = 表项内容.FindAll( t => t.符号名 == 符号名 );
                if (find.Count==0 && 外表 != null) {
                    return 外表[符号名];
                }
                return find;
            }
        }
        public void 添加(符号表项 表项) {
            表项内容.Add(表项);
        }
        //符号表项
        internal abstract class 符号表项 {
            //符号名 类别
            public enum 表项类别Enum {
                过程,//形参类型表
                类型,//类型描述
                变量,//类型
            }
            public string 符号名 { get; }
            public int 行号 { get; }
            public 符号表 表 { get; }
            public 表项类别Enum 表项类别 { get; }
            public 符号表项(
                string 符号名,
                int 行号,
                符号表 表,
                表项类别Enum 表项类别
            ) {
                this.符号名 = 符号名;
                this.行号 = 行号;
                this.表 = 表;
                this.表项类别 = 表项类别;
            }
        }
        //变量
        internal class 变量 : 符号表项 {
            public 类型描述 类型 { get; }
            public bool 参数 { get; init; } = false;
            public 变量(
                string 变量名,
                int 行号,
                符号表 表,
                类型描述 类型,
                bool 参数=false
            ) : base(
                变量名, 行号, 表,
                表项类别Enum.变量
            ) {
                this.类型 = 类型;
                this.参数 = 参数;
            }
        }
        //过程
        internal class 过程 : 符号表项 {
            public List<(变量声明 参数, bool 引用)> 参数列表 { get; init; }
            public 过程(
                string 过程名,
                int 行号,
                符号表 表,
                List<(变量声明 参数, bool 引用)>? 参数列表 = null
            ) : base(
                过程名, 行号, 表,
                表项类别Enum.过程
            ) {
                this.参数列表 = 参数列表 ?? new();
            }
        }
        //类型
        internal class 类型 : 符号表项 {
            /*
            ** 这里的类型都是自定义的
            ** 新类型=旧类型 的形式
            ** 旧类型可能是基础、数组、记录
            ** 也可能是别的“新”类型
            ** 描述里面存的一定是旧类型
            */
            类型描述 描述;
            public 类型(
                string 类型名,
                int 行号,
                符号表 表,
                类型描述 描述
            ) : base(
                类型名, 行号, 表,
                表项类别Enum.类型
            ) {
                this.描述 = 描述;
            }
        }
    }
}
