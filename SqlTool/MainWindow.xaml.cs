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

namespace SqlTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        int paramCount = 0;
        Dictionary<int, List<string>> dic_param = new Dictionary<int, List<string>>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void addParam_Click(object sender, RoutedEventArgs e)
        {
            grid.RowDefinitions.Add(new RowDefinition());

            int row = Convert.ToInt32(btn_list.GetValue(Grid.RowProperty));
            btn_list.SetValue(Grid.RowProperty, row + 1);

            txt_result.SetValue(Grid.RowProperty, Convert.ToInt32(txt_result.GetValue(Grid.RowProperty)) + 1);

            var paramLb = new Label();
            paramLb.Content = $"参数{++paramCount}";
            grid.Children.Add(paramLb);
            paramLb.SetValue(Grid.RowProperty, row - 1);

            var paramtxt = new TextBox();
            paramtxt.Name = $"param{paramCount}";
            paramtxt.AcceptsReturn = true;
            paramtxt.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            paramtxt.TextWrapping = TextWrapping.Wrap;
            grid.Children.Add(paramtxt);
            paramtxt.SetValue(Grid.RowProperty, row - 1);
            paramtxt.SetValue(Grid.ColumnProperty, 1);
        }

        private void build_Click(object sender, RoutedEventArgs e)
        {
            var children = grid.Children;
            dic_param = new Dictionary<int, List<string>>();
            foreach (var c in children)
            {
                if (c is TextBox)
                {
                    var txt = (TextBox)c;
                    if (txt.Name.Contains("param"))
                    {
                        var num = Convert.ToInt32(txt.Name.Substring(5, txt.Name.Length - 5));
                        dic_param.Add(num, txt.Text.Replace("\r\n",",").Split(',').ToList());
                    }
                }
            }

            int paramlength = -1;
            foreach (var kv in dic_param)
            {
                if (paramlength == -1)
                {
                    paramlength = kv.Value.Count;
                }
                else if (paramlength != kv.Value.Count)
                {
                    txt_result.Text = "Error";
                }
            }

            StringBuilder sb = new StringBuilder(500);
            for (int i = 0; i < paramlength; i++)
            {
                var ss = new List<string>();
                for (var j = 1; j <= paramCount; j++)
                {
                    ss.Add(dic_param[j][i]);
                }
                sb.AppendFormat(txt_sql.Text, ss.ToArray());
                sb.Append("\r\n");
            }
            txt_result.Text = sb.ToString();
        }
    }
}
