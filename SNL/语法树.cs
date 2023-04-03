using System;
using System.Collections.Generic;

namespace SNL {
    using static SNL.符号表;
    using 参数列表类型 = List<(类型描述 类型, bool 引用)>;
    internal class 语法树 {
        public 过程声明 主程序 { get; }
        public 语法树(过程声明 主程序) => this.主程序 = 主程序;
        public List<语义错误> 语义检查() => 主程序.语义检查();
        public void 更新符号表() => 主程序.更新符号表();
        //语法树结点
        internal abstract class 语法点 {
            public 语法点? 亲 { get; }
            public int 行号 { get; } = 0;
            public virtual 符号表 局部符号表 => 亲!.局部符号表;
            public abstract List<语义错误> 语义检查();
            public 语法点(int 行号, 语法点? 亲 = null) {
                this.行号 = 行号;
                this.亲 = 亲;
            }
            public string Indent {
                get {
                    return 亲 == null ? "" : 亲.Indent + "\t";
                }
            }
        }
        //语法之声明
        internal abstract class 声明 : 语法点 {
            public abstract void 更新符号表();
            public 声明(int 行号, 语法点? 亲) : base(行号, 亲) { }
        }
        //语法之类型声明
        internal class 类型声明 : 声明 { 
            public override void 更新符号表() => 局部符号表.添加(new 符号表.类型(类型名, 行号, 局部符号表, 类型定义));
            public override List<语义错误> 语义检查() {
                var 语义错误列表 = new List<语义错误>();


                switch (类型定义.类型类别) {
                    case 类型描述.类型类别Enum.数组类型:
                        var a = (类型定义 as 数组类型描述)!;
                        if (a.下界 >= a.上界) {
                            语义错误列表.Add(new 语义错误 {
                                行号 = 行号,
                                错误内容 = 语义错误Enum.数组下标非法
                            });
                        }
                        break;
                    case 类型描述.类型类别Enum.记录类型:
                        var r = (类型定义 as 记录类型描述)!;

                        break;
                    case 类型描述.类型类别Enum.自定类型:
                        var s = (类型定义 as 自定类型描述)!;
                        switch (局部符号表[s.类型名].Count) {
                            case 0:
                                语义错误列表.Add(new 语义错误 {
                                    行号 = 行号,
                                    错误内容 = 语义错误Enum.未知的类型
                                });
                                break;
                            case 1:
                                break;
                            default:
                                语义错误列表.Add(new 语义错误 {
                                    行号 = 行号,
                                    错误内容 = 语义错误Enum.标识符重复定义
                                });
                                break;
                        }
                        break;
                }

                return 语义错误列表;
            }
            public override string ToString() {
                string content = "";
                content += $"{Indent}{类型名}\n";
                foreach (var s in 类型定义.ToString()!.Split('\n')) {
                    content += $"{Indent}{s}\n";
                }
                return content;
            }
            /***************************/
            public string 类型名 { get; init; }
            public 类型描述 类型定义 { get; init; }
            public 类型声明(int 行号, 语法点 亲, string 类型名, 类型描述 类型定义) :
                base(行号, 亲) {
                this.类型名 = 类型名;
                this.类型定义 = 类型定义;
            }
        }
        //语法之变量声明
        internal class 变量声明 : 声明 { 
            public override void 更新符号表() => 局部符号表.添加(new 符号表.变量(变量名, 行号, 局部符号表, 变量类型));
            public override List<语义错误> 语义检查() {
                var 语义错误列表 = new List<语义错误>();
                if (变量类型.类型类别 == 类型描述.类型类别Enum.自定类型) {
                    var t = 局部符号表[(变量类型 as 自定类型描述)!.类型名];
                    if (t == null) {
                        语义错误列表.Add(new 语义错误 {
                            行号 = 行号,
                            错误内容 = 语义错误Enum.未知的类型
                        });
                    }
                }
                return 语义错误列表;
            }
            public override string ToString() {
                string content = "";
                content += $"{Indent}{变量名}\n";
                foreach (var s in 变量类型.ToString()!.Split('\n')) {
                    content += $"{Indent}{s}\n";
                }
                return content;
            }

            /***************************/
            public string 变量名 { get; }
            public 类型描述 变量类型 { get; }
            public 变量声明(
                int 行号,
                语法点 亲,
                string 变量名,
                类型描述 变量类型
            ) : base(
                行号,
                亲
            ) {
                this.变量名 = 变量名;
                this.变量类型 = 变量类型;
            }
        }
        //语法之过程声明
        internal class 过程声明 : 声明 {
            符号表 _局部符号表 = new();
            public override 符号表 局部符号表 => _局部符号表;
            public override void 更新符号表() => 亲?.局部符号表.添加(new 过程(过程名, 行号, 亲!.局部符号表, 参数列表));
            public override List<语义错误> 语义检查() {
                var 语义错误列表 = new List<语义错误>();

                return 语义错误列表;
            }
            public override string ToString() {
                string content = "";
                content += $"{Indent}过程：{过程名}\n";
                content += $"{Indent}参数列表\n";
                foreach (var 参数 in 参数列表) {
                    content += $"{Indent}\t{(参数.引用 ? "VAR" : "VAL")}";
                    content += 参数.参数.ToString();
                }
                content += $"{Indent}变量列表\n";
                foreach (var 变量 in 变量声明列表) {
                    content += 变量.ToString();
                }
                content += $"{Indent}过程列表\n";
                foreach (var 过程 in 过程声明列表) {
                    content += 过程.ToString();
                }
                content += $"{Indent}过程体\n";
                foreach (var 语句 in 过程体) {
                    content += 语句.ToString();
                }
                return content;
            }
            /*****************/
            public string 过程名 { get; }
            public List<(变量声明 参数, bool 引用)> 参数列表 { get; init; } = new();
            public List<类型声明> 类型声明列表 { get; init; } = new();
            public List<变量声明> 变量声明列表 { get; init; } = new();
            public List<过程声明> 过程声明列表 { get; init; } = new();
            public List<语句> 过程体 { get; init; } = new();
            /*****************/
            internal 过程声明(
                int 行号,
                语法点? 亲,
                string 过程名,
                参数列表类型? 参数列表 = null
            ) : base(
                行号,
                亲
            ) {
                this.过程名 = 过程名;
                this._局部符号表.添加(new 符号表.过程(
                    过程名,
                    行号,
                    _局部符号表,
                    参数列表
                ));
            }
        }
        //语法之表达式
        internal class 表达式 : 语法点 { 
            public override List<语义错误> 语义检查() {
                var 语义错误列表 = new List<语义错误>();

                return 语义错误列表;
            }
            public override string ToString() {
                return ($"{Indent}{算符}{内容}\n")
                    + (左?.ToString() ?? "")
                    + (右?.ToString() ?? "");
            }
            /****************/
            /*
            ** # 立即数
            ** $ 变量
            ** + 加法
            ** - 减法
            ** * 乘法
            ** / 除法
            ** < 小于
            ** = 等于
            ** [ 数组访问
            ** . 成员访问
            */

            public char 算符 { get; }
            //为变量或立即数时有效
            public string 内容 { get; }
            public 表达式? 左 { get; set; }
            public 表达式? 右 { get; set; }
            public 表达式(
                int 行号,
                语法点 亲,
                char 算符,
                string 内容 = ""
            ) : base(
                行号,
                亲
            ) {
                this.算符 = 算符;
                this.内容 = 内容;
            }
        }
        //语法之语句
        internal abstract class 语句 : 语法点 {
            public enum 语句类别Enum {
                空的语句,
                赋值语句,
                调用语句,
                条件语句,
                循环语句,
                输入语句,
                输出语句,
                返回语句,
            } 
            /*********************/
            public 语句类别Enum 语句类别 { get; }
            public 语句(
                int 行号,
                语法点 亲,
                语句类别Enum 语句类别
            ) : base(
                行号, 亲
            ) {
                this.语句类别 = 语句类别;
            }
        }
        //语法之语句之空的
        internal class 空的语句 : 语句 {
            public override List<语义错误> 语义检查() {
                return new();
            }
            public override string ToString() {
                return "";
            }
            /******************/
            public 空的语句(int 行号, 语法点 亲) : base(行号, 亲, 语句类别Enum.空的语句) { }
        }
        //语法之语句之调用
        internal class 调用语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();
                return 语义错误列表;
            }
            public override string ToString() {
                string content = "";
                content += $"{Indent}调用：{过程名}\n";
                foreach (var exp in 实参列表) {
                    content += exp.ToString();
                }
                return content;
            }
            /******************/
            public string 过程名 { get; }
            public List<表达式> 实参列表 { get; } = new();
            public 调用语句(int 行号, 语法点 亲, string 过程名) : base(行号, 亲, 语句类别Enum.调用语句) { this.过程名 = 过程名; }
        }
        //语法之语句之赋值
        internal class 赋值语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();
                return 语义错误列表;
            }
            public override string ToString() {
                return $"{Indent}赋值\n" + 左!.ToString() + 右!.ToString();
            }
            /*********************/
            public 表达式? 左 { get; set; }
            public 表达式? 右 { get; set; }
            public 赋值语句(int 行号, 语法点 亲) : base(行号, 亲, 语句类别Enum.赋值语句) { }
        }
        //语法之语句之输出
        internal class 输出语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();
                return 语义错误列表;
            }
            public override string ToString() {
                return $"{Indent}输出\n" + 待输出值!.ToString();
            }
            /*********************/
            public 表达式? 待输出值 { get; set; }
            public 输出语句(int 行号, 语法点 亲) : base(行号, 亲, 语句类别Enum.输出语句) { }
        }
        //语法之语句之输入
        internal class 输入语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();
                return 语义错误列表;
            }
            public override string ToString() {
                return $"{Indent}输入\n" + 待输入量!.ToString();
            }
            /*********************/
            public 表达式? 待输入量 { get; set; } = null;
            public 输入语句(int 行号, 语法点 亲) : base(行号, 亲, 语句类别Enum.输入语句) { }
        }
        //语法之语句之条件
        internal class 条件语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();
                return 语义错误列表;
            }
            public override string ToString() {
                string content = "";
                content += $"{Indent}条件\n";
                content += 条件!.ToString();
                content += $"{Indent}THEN\n";
                foreach (var stm in THEN) {
                    content += stm.ToString();
                }
                content += $"{Indent}ELSE\n";
                foreach (var stm in ELSE) {
                    content += stm.ToString();
                }
                return content;
            }
            /******************/
            public 表达式? 条件 { get; set; } = null;
            public List<语句> THEN { get; } = new();
            public List<语句> ELSE { get; } = new();
            public 条件语句(int 行号, 语法点 亲) : base(行号, 亲, 语句类别Enum.条件语句) { }
        }
        //语法之语句之循环
        internal class 循环语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();
                return 语义错误列表;
            }
            public override string ToString() {
                string content = "";
                content += $"{Indent}循环\n";
                content += 条件!.ToString();
                content += $"{Indent}DO\n";
                foreach (var stm in 循环体) {
                    content += stm.ToString();
                }
                return content;
            }
            /******************/
            public 表达式? 条件 { get; set; } = null;
            public List<语句> 循环体 { get; } = new();
            public 循环语句(int 行号, 语法点 亲) : base(行号, 亲, 语句类别Enum.条件语句) { }
        }
        //语法之语句之返回语句
        internal class 返回语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();
                return 语义错误列表;
            }
            public override string ToString() {
                return $"{Indent}返回";
            }
            /******************/
            public 返回语句(int 行号, 语法点 亲) : base(行号, 亲, 语句类别Enum.返回语句) { }
        }
    }
}
