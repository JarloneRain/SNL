using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SNL {
    public partial class MainWindow : Window {
        internal enum 编译状态Enum {
            请先开始编译,
            词法分析出错,
            词法分析完成,
            语法分析出错,
            语法分析完成,
            语义分析出错,
            语义分析完成,
        }
        编译状态Enum _编译状态;
        internal 编译状态Enum 编译状态 {
            get { return _编译状态; }
            set { _编译状态 = value; 文本块编译状态.Text = _编译状态.ToString(); }
        }
        internal List<Token> TokenList { get; set; } = new();
        internal 语法树? 语法树;

        public MainWindow() {
            InitializeComponent();
            文本框代码.Text = @"program bubble {程序头 程序名标识符}
    type
        rt=record integer i;char c;end;
    var
        integer i,j,num;
        char ch;
        array[1..20] of integer a;
        rt r;
    procedure q(integer num);
        var
        integer i,j,k;
        integer t;    
    begin        
        i:=1;        
        j:=1;        
        while i < num do 
            j:=num-i+1;
            k:=1;
            while k<j do
                if a[k+1] < a[k] then 
                    t:=a[k];
                    a[k]:=a[k+1];
                    a[k+1]:=t
                else 
                    t:=0
                fi;
                k:=k+1
            endwh;
            i:=i+1
        endwh
    end
begin
    read(ch);
    r.c:=ch;
    r.i:=1;
    read(num);
    i:=1;
    while i<(num+1) do
        read(j);
        a[i]:=j;
        i:=i+1
    endwh;
    q(num);
    i:=1;
    while i<(num+1) do
        write(a);
        i:=i+1
    endwh
end.";
            文本块编译状态.Text = 编译状态Enum.请先开始编译.ToString();
            
        }

        private void 按钮词法分析_Click(object sender, RoutedEventArgs e) {
            try {
                TokenList = 词法.分析(文本框代码.Text + "~");
                编译状态 = 编译状态Enum.词法分析完成;
                文本框编译结果.Text = "";
                foreach (Token token in TokenList) {
                    文本框编译结果.Text += $"{token}\n";
                }
            } catch (Exception ex) {
                编译状态 = 编译状态Enum.词法分析出错;
                文本框编译结果.Text = ex.Message;
            }
        }

        private void 按钮语法分析_Click(object sender, RoutedEventArgs e) {
            按钮词法分析_Click(sender, e);
            if (编译状态 != 编译状态Enum.词法分析完成) {
                MessageBox.Show("词法分析出错！");
                return;
            }
            try {
                switch (下拉语法分析方式.SelectedIndex) {
                    case 0:
                        MessageBox.Show("将开始递归下降语法分析！");
                        break;
                    case 1:
                        语法树 = 语法.分析_LL1(TokenList);
                        break;
                }
                文本框编译结果.Text = 语法树!.ToString();
                编译状态 = 编译状态Enum.语法分析完成;
            } catch (语法分析异常 ex) {
                文本框编译结果.Text = ex.Message;
            }
        }

        private void 按钮语义分析_Click(object sender, RoutedEventArgs e) {
            按钮语法分析_Click(sender, e);
            if (编译状态 != 编译状态Enum.语法分析完成) {
                MessageBox.Show("语法分析出错！");
                return;
            }
            try {
                var 语义错误列表 = 语义.分析(语法树!);
                文本框编译结果.Text = 语义错误列表.Count == 0 ? "没有语义错误" : "";
                语义错误列表.ForEach(e => 文本框编译结果.Text += $"{e}\n");
                编译状态 = 编译状态Enum.语义分析完成;
            } catch (语法分析异常 ex) {
                编译状态 = 编译状态Enum.语义分析出错;
                文本框编译结果.Text = ex.Message;
            }
        }
    }
}
