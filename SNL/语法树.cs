using System.Collections.Generic;

namespace SNL {
    internal class 语法树 {
        public 过程声明 主程序 { get; }
        public 语法树(过程声明 主程序) => this.主程序 = 主程序;
        public List<语义错误> 语义检查() => 主程序.语义检查();
        public void 更新符号表() => 主程序.更新符号表();
        public override string ToString() => 主程序!.ToString();
        static 语义错误 符号数目错误(string 符号, int 行号, 符号表 局部符号表) {
            switch (局部符号表[符号].Count) {
                case 0:
                    return new 语义错误 {
                        行号 = 行号,
                        错误类别 = 语义错误.语义错误类别Enum.未声明标识符,
                        附加说明 = 符号,
                    };
                case 1:
                    return 语义错误.OK;
                default:
                    return new 语义错误 {
                        行号 = 行号,
                        错误类别 = 语义错误.语义错误类别Enum.重复的标识符,
                        附加说明 = 符号,
                    };
            }
        }
        //语法树结点
        internal abstract class 语法点 {
            public 语法点? 亲 { get; }
            public int 行号 { get; } = 0;
            public virtual 符号表 局部符号表 => 亲!.局部符号表;
            public abstract List<语义错误> 语义检查();
            public string Indent => 亲 == null ? "" : 亲.Indent + "\t";
            public 语法点(int 行号, 语法点? 亲 = null) {
                this.行号 = 行号;
                this.亲 = 亲;
            }

        }
        //语法之声明
        internal abstract class 声明 : 语法点 {
            public abstract void 更新符号表();
            public 声明(int 行号, 语法点? 亲) :
                base(行号, 亲) {
            }
        }
        //语法之类型声明
        internal class 类型声明 : 声明 {
            public override void 更新符号表() => 局部符号表.添加(new 符号表.类型(类型名, 行号, 局部符号表, 类型定义));
            public override List<语义错误> 语义检查() {
                var 语义错误列表 = new List<语义错误>();
                switch (类型定义.类型类别) {
                    case 类型描述.类型类别Enum.数组类型:
                        (类型定义 as 数组类型描述)!.语义检查().ForEach(e => 语义错误列表.Add(e));
                        break;
                    case 类型描述.类型类别Enum.记录类型:
                        (类型定义 as 记录类型描述)!.语义检查().ForEach(e => 语义错误列表.Add(e));
                        break;
                    case 类型描述.类型类别Enum.自定类型:
                        var s = (类型定义 as 自定类型描述)!;
                        语义错误列表.Add(符号数目错误(s.类型名, s.行号, 局部符号表));
                        break;
                }

                语义错误列表.Add(符号数目错误(类型名, 行号, 局部符号表));

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
                    var t = (变量类型 as 自定类型描述)!;
                    语义错误列表.Add(符号数目错误(t.类型名, 行号, 局部符号表));
                } else {
                    变量类型.语义检查().ForEach(e => 语义错误列表.Add(e));
                }
                语义错误列表.Add(符号数目错误(变量名, 行号, 局部符号表));

                return 语义错误列表;
            }
            public override string ToString() {
                string content = "";
                content += $"{Indent}{变量名}\n";
                foreach (var s in 变量类型.ToString()!.Split('\n')) {
                    content += $"{Indent}\t{s}\n";
                }
                return content;
            }

            /***************************/
            public string 变量名 { get; }
            public 类型描述 变量类型 { get; }
            public 变量声明(int 行号, 语法点 亲, string 变量名, 类型描述 变量类型) :
                base(行号, 亲) {
                this.变量名 = 变量名;
                this.变量类型 = 变量类型;
            }
        }
        //语法之过程声明
        internal class 过程声明 : 声明 {
            符号表 _局部符号表;
            public override 符号表 局部符号表 => _局部符号表;
            public override void 更新符号表() {
                局部符号表.添加(new 符号表.过程(过程名, 行号, _局部符号表, 参数列表));
                参数列表.ForEach(a => 局部符号表.添加(new 符号表.变量(a.参数.变量名, a.参数.行号, 局部符号表, a.参数.变量类型, true)));
                类型声明列表.ForEach(t => t.更新符号表());
                变量声明列表.ForEach(v => v.更新符号表());
                过程声明列表.ForEach(p => {
                    局部符号表.添加(new 符号表.过程(p.过程名, p.行号, _局部符号表, p.参数列表));
                    p.更新符号表();
                });
            }
            public override List<语义错误> 语义检查() {
                var 语义错误列表 = new List<语义错误>() {
                    符号数目错误(过程名, 行号, 局部符号表)
                };

                参数列表.ForEach(a => a.参数.语义检查().ForEach(e => 语义错误列表.Add(e)));
                类型声明列表.ForEach(t => t.语义检查().ForEach(e => 语义错误列表.Add(e)));
                变量声明列表.ForEach(v => v.语义检查().ForEach(e => 语义错误列表.Add(e)));
                过程声明列表.ForEach(p => p.语义检查().ForEach(e => 语义错误列表.Add(e)));
                过程体.ForEach(s => s.语义检查().ForEach(e => 语义错误列表.Add(e)));

                return 语义错误列表;
            }
            public override string ToString() {
                string content = "";
                content += $"{Indent}过程：{过程名}\n";
                content += $"{Indent}参数列表\n";
                foreach (var 参数 in 参数列表) {
                    content += $"{Indent}\t{(参数.引用 ? "VAR" : "VAL")}\n";
                    content += 参数.参数.ToString();
                }
                content += $"{Indent}类型列表\n";
                foreach (var 类型 in 类型声明列表) {
                    content += 类型.ToString();
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
            internal 过程声明(int 行号, 语法点? 亲, string 过程名, List<(变量声明 参数, bool 引用)>? 参数列表 = null) :
                base(行号, 亲) {
                this.过程名 = 过程名;
                _局部符号表 = new 符号表(亲?.局部符号表);
            }
        }
        //语法之表达式
        internal class 表达式 : 语法点 {
            public override List<语义错误> 语义检查() {
                var 语义错误列表 = new List<语义错误>();
                switch (算符) {
                    case '$':
                        语义错误列表.Add(符号数目错误(内容, 行号, 局部符号表));
                        break;
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '<':
                    case '=':
                        左!.语义检查().ForEach(e => 语义错误列表.Add(e));
                        右!.语义检查().ForEach(e => 语义错误列表.Add(e));
                        if (!左!.是数值() || !右!.是数值()) {
                            语义错误列表.Add(new 语义错误 {
                                行号 = 行号,
                                错误类别 = 语义错误.语义错误类别Enum.运算符的分量类型不相容,
                                附加说明 = $"{this}",
                            });
                        }
                        break;
                    case '_':
                        if (!左!.表达式类型().是数组()) {
                            语义错误列表.Add(new 语义错误 {
                                行号 = 行号,
                                错误类别 = 语义错误.语义错误类别Enum.非期望的标识符类别,
                                附加说明 = $"{左}"
                            });
                        }
                        if (!右!.是数值()) {
                            语义错误列表.Add(new 语义错误 {
                                行号 = 行号,
                                错误类别 = 语义错误.语义错误类别Enum.数组下标非法,
                                附加说明 = $"{右}",
                            });
                        }
                        break;
                    case '.':
                        var t = 左!.表达式类型();
                        if (!左!.表达式类型().是记录()) {
                            语义错误列表.Add(new 语义错误 {
                                行号 = 行号,
                                错误类别 = 语义错误.语义错误类别Enum.非期望的标识符类别,
                                附加说明 = $"{左}",
                            });
                        }
                        var r = (t as 记录类型描述)!;
                        if (r.字段.FindAll(f => f.名称 == 右!.内容).Count != 1) {
                            语义错误列表.Add(new 语义错误 {
                                行号 = 行号,
                                错误类别 = 语义错误.语义错误类别Enum.未知的字段,
                                附加说明 = $"{右}",
                            });
                        }
                        break;
                }
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
            ** . 成员访问
            ** _ 数组访问
            ** + 加法
            ** - 减法
            ** * 乘法
            ** / 除法
            ** < 小于
            ** = 等于
            */
            public char 算符 { get; }
            //为变量或立即数时有效
            public string 内容 { get; }
            public 表达式? 左 { get; set; }
            public 表达式? 右 { get; set; }
            public bool 是左值() {
                switch (算符) {
                    case '$':
                        var vs = 局部符号表[内容];
                        if (vs.Count != 1) {
                            return false;
                        }
                        var v = vs[0];
                        return v.表项类别 == 符号表.符号表项.表项类别Enum.变量
                            && (v as 符号表.变量)!.类型.类型类别 == 类型描述.类型类别Enum.基础类型;
                    case '.':
                        var rs = 局部符号表[左!.内容];
                        if (rs.Count != 1) {
                            return false;
                        }
                        var r = rs[0];
                        if (r.表项类别 != 符号表.符号表项.表项类别Enum.变量
                            || (r as 符号表.变量)!.类型.获取原型(局部符号表).类型类别 != 类型描述.类型类别Enum.记录类型) {
                            return false;
                        }
                        var fs = ((r as 符号表.变量)!.类型.获取原型(局部符号表) as 记录类型描述)!.字段.FindAll(f => f.名称 == 右!.内容);
                        if (fs.Count != 1) {
                            return false;
                        }
                        var f = fs[0];
                        if (f.类型.类型类别 != 类型描述.类型类别Enum.基础类型) {
                            return false;
                        }
                        return true;
                    case '_':
                        return 左!.表达式类型().是数组()
                            && 右!.是数值();
                    default: return false;
                }
            }
            public bool 是布尔() => 表达式类型().是布尔();
            public bool 是数值() {
                switch (算符) {
                    case '#':
                        return true;
                    case '$':
                    case '.':
                    case '_':
                        return 是左值();
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                        return 左!.是数值() && 右!.是数值();
                    case '<':
                    case '=':
                        return false;
                    default:
                        return false;
                }
            }
            public 类型描述 表达式类型() {
                switch (算符) {
                    case '#':
                        return 基础类型描述.整数类型();
                    case '$':
                        var vs = 局部符号表[内容];
                        if (vs.Count != 1) {
                            return 类型描述.未知类型;
                        }
                        var v = vs[0];
                        if (v.表项类别 != 符号表.符号表项.表项类别Enum.变量) {
                            return 类型描述.未知类型;
                        }
                        return (v as 符号表.变量)!.类型.获取原型(局部符号表);
                    case '.':
                        var rs = 局部符号表[左!.内容];
                        if (rs.Count != 1) {
                            return 类型描述.未知类型;
                        }
                        var r = rs[0];
                        if (rs[0].表项类别 != 符号表.符号表项.表项类别Enum.变量) {
                            return 类型描述.未知类型;
                        }
                        var ro = (r as 符号表.变量)!.类型.获取原型(局部符号表);
                        if (ro.类型类别 != 类型描述.类型类别Enum.记录类型) {
                            return 类型描述.未知类型;
                        }
                        var fs = (ro as 记录类型描述)!.字段.FindAll(f => f.名称 == 右!.内容);
                        if (fs.Count != 1) {
                            return 类型描述.未知类型;
                        }
                        var f = fs[0];
                        return f.类型;
                    case '_':
                        var a = 左!.表达式类型();
                        if (!a.是数组()) {
                            return 类型描述.未知类型;
                        }
                        var k = 右!.表达式类型();
                        if (!k.是整数()) {
                            return 类型描述.未知类型;
                        }
                        return (a as 数组类型描述)!.元素类型;
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                        if (左!.是数值() && 右!.是数值()) {
                            return 基础类型描述.整数类型();
                        }
                        return 类型描述.未知类型;
                    case '<':
                    case '=':
                        if (左!.是数值() && 右!.是数值()) {
                            return 基础类型描述.布尔类型();
                        }
                        return 类型描述.未知类型;
                    default:
                        return 类型描述.未知类型;
                }
            }
            public 表达式(int 行号, 语法点 亲, char 算符, string 内容 = "") :
                base(行号, 亲) {
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
            public 语句(int 行号, 语法点 亲, 语句类别Enum 语句类别) :
                base(行号, 亲) {
                this.语句类别 = 语句类别;
            }
        }
        //语法之语句之空的
        internal class 空的语句 : 语句 {
            public override List<语义错误> 语义检查() => new();
            public override string ToString() => "";
            /******************/
            public 空的语句(int 行号, 语法点 亲) :
                base(行号, 亲, 语句类别Enum.空的语句) {
            }
        }
        //语法之语句之调用
        internal class 调用语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();

                var ps = 局部符号表[过程名];
                if (ps.Count != 1) {
                    语义错误列表.Add(符号数目错误(过程名, 行号, 局部符号表));
                } else {
                    var p = ps[0];
                    if (p.表项类别 != 符号表.符号表项.表项类别Enum.过程) {
                        语义错误列表.Add(new 语义错误 {
                            行号 = 行号,
                            错误类别 = 语义错误.语义错误类别Enum.非期望的标识符类别,
                            附加说明 = 过程名,
                        });
                    } else {
                        var proc = (p as 符号表.过程)!;
                        if (proc.参数列表.Count != 实参列表.Count) {
                            语义错误列表.Add(new 语义错误 {
                                行号 = 行号,
                                错误类别 = 语义错误.语义错误类别Enum.参数数目错误,
                                附加说明 = 过程名,
                            });
                        } else {
                            for (int k = 0; k < (proc.参数列表.Count + 实参列表.Count) / 2; k++) {
                                var arg = proc.参数列表[k];
                                var act = 实参列表[k];
                                if (arg.引用 && !act.是左值() ||
                                    !arg.参数.变量类型.获取原型(局部符号表).能接受(act.表达式类型())
                                ) {
                                    语义错误列表.Add(new 语义错误 {
                                        行号 = act.行号,
                                        错误类别 = 语义错误.语义错误类别Enum.参数类型错误,
                                        附加说明 = $"{act}",
                                    });
                                }
                            }
                        }
                    }
                }

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
            public 调用语句(int 行号, 语法点 亲, string 过程名) :
                base(行号, 亲, 语句类别Enum.调用语句) {
                this.过程名 = 过程名;
            }
        }
        //语法之语句之赋值
        internal class 赋值语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();

                if (!左!.是左值()) {
                    语义错误列表.Add(new 语义错误 {
                        行号 = 左!.行号,
                        错误类别 = 语义错误.语义错误类别Enum.向非变量的标识符赋值,
                        附加说明 = $"{左}",
                    });
                }
                if (!左!.表达式类型().能接受(右!.表达式类型())) {
                    语义错误列表.Add(new 语义错误 {
                        行号 = 右!.行号,
                        错误类别 = 语义错误.语义错误类别Enum.赋值语句的左右两边类型不相容,
                        附加说明 = $"{this}",
                    });
                }

                return 语义错误列表;
            }
            public override string ToString() => $"{Indent}赋值\n{左!}{右!}";
            /*********************/
            public 表达式? 左 { get; set; }
            public 表达式? 右 { get; set; }
            public 赋值语句(int 行号, 语法点 亲) : base(行号, 亲, 语句类别Enum.赋值语句) { }
        }
        //语法之语句之输出
        internal class 输出语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();

                待输出值!.语义检查().ForEach(e => 语义错误列表.Add(e));

                return 语义错误列表;
            }
            public override string ToString() => $"{Indent}输出\n{待输出值!}";
            /*********************/
            public 表达式? 待输出值 { get; set; }
            public 输出语句(int 行号, 语法点 亲) :
                base(行号, 亲, 语句类别Enum.输出语句) {
            }
        }
        //语法之语句之输入
        internal class 输入语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();

                待输入量!.语义检查().ForEach(e => 语义错误列表.Add(e));
                if (!待输入量!.是左值()) {
                    语义错误列表.Add(new 语义错误 {
                        行号 = 行号,
                        错误类别 = 语义错误.语义错误类别Enum.向非变量的标识符赋值,
                        附加说明 = $"{待输入量}",
                    });
                }

                return 语义错误列表;
            }
            public override string ToString() => $"{Indent}输入\n{待输入量!}";
            /*********************/
            public 表达式? 待输入量 { get; set; } = null;
            public 输入语句(int 行号, 语法点 亲) : base(行号, 亲, 语句类别Enum.输入语句) { }
        }
        //语法之语句之条件
        internal class 条件语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();

                if (!条件!.是布尔()) {
                    语义错误列表.Add(new() {
                        行号 = 条件!.行号,
                        错误类别 = 语义错误.语义错误类别Enum.非布尔的条件表达式,
                        附加说明 = $"{条件}",
                    });
                }

                THEN.ForEach(s => s.语义检查().ForEach(e => 语义错误列表.Add(e)));
                ELSE.ForEach(s => s.语义检查().ForEach(e => 语义错误列表.Add(e)));

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
            public 条件语句(int 行号, 语法点 亲) :
                base(行号, 亲, 语句类别Enum.条件语句) {
            }
        }
        //语法之语句之循环
        internal class 循环语句 : 语句 {
            public override List<语义错误> 语义检查() {
                List<语义错误> 语义错误列表 = new();

                if (!条件!.是布尔()) {
                    语义错误列表.Add(new() {
                        行号 = 条件!.行号,
                        错误类别 = 语义错误.语义错误类别Enum.非布尔的条件表达式,
                        附加说明 = $"{条件}",
                    });
                }

                循环体.ForEach(s => s.语义检查().ForEach(e => 语义错误列表.Add(e)));

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
            public 循环语句(int 行号, 语法点 亲) :
                base(行号, 亲, 语句类别Enum.循环语句) {
            }
        }
        //语法之语句之返回语句
        internal class 返回语句 : 语句 {
            public override List<语义错误> 语义检查() => new();
            public override string ToString() => $"{Indent}返回";
            /******************/
            public 返回语句(int 行号, 语法点 亲) :
                base(行号, 亲, 语句类别Enum.返回语句) {
            }
        }
    }
}