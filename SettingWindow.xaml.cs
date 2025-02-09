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
using System.Windows.Shapes;

namespace Deepseek_Desktop_App
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            APIkey.Text=Properties.Settings.Default.APIkey;
            APIurl.Text=Properties.Settings.Default.APIurl;
            model.Text=Properties.Settings.Default.model;
            max_tokens.Text= Properties.Settings.Default.max_tokens.ToString();
            temperature.Text= Properties.Settings.Default.temperature.ToString();
            ChatFontSize.Text=Properties.Settings.Default.ChatFontSize.ToString();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            
            

            int max_tokensOut, ChatFontSizeOut = 0;
            float temperatureOut = 0;
            
            //检查是否合法
            if(int.TryParse(max_tokens.Text, out max_tokensOut) && int.TryParse(ChatFontSize.Text, out ChatFontSizeOut) && float.TryParse(temperature.Text, out temperatureOut)) Console.WriteLine("设定数据合法");
            if (max_tokensOut <= 4096 && ChatFontSizeOut <= 30 && temperatureOut <= 1.5) Console.WriteLine("设定数据范围合法");
            else
            {
                string WrongMessage = "数据错误：";
                if (!int.TryParse(max_tokens.Text, out max_tokensOut) || max_tokensOut > 4096) WrongMessage += "\nmax_tokens只支持4096或以下整数";
                if (!int.TryParse(ChatFontSize.Text, out ChatFontSizeOut) || ChatFontSizeOut > 30) WrongMessage += "\n聊天字体大小只支持30或以下整数";
                if (!float.TryParse(temperature.Text, out temperatureOut) || temperatureOut > 1.5) WrongMessage += "\ntemperature只支持1.5或以下一位小数";
                System.Windows.MessageBox.Show(WrongMessage, "数据错误", MessageBoxButton.OK, MessageBoxImage.Warning);//弹出提示框
                return;//提前结束进程
            }
            //检查变化并设定
            if (Properties.Settings.Default.APIkey != APIkey.Text) Properties.Settings.Default.APIkey = APIkey.Text;
            if (Properties.Settings.Default.APIurl != APIurl.Text) Properties.Settings.Default.APIurl = APIurl.Text;
            if (Properties.Settings.Default.model != model.Text) Properties.Settings.Default.model = model.Text;
            if (Properties.Settings.Default.max_tokens.ToString() != max_tokens.Text) Properties.Settings.Default.max_tokens= max_tokensOut;
            if (Properties.Settings.Default.temperature.ToString() != temperature.Text) Properties.Settings.Default.temperature = temperatureOut;
            if (Properties.Settings.Default.ChatFontSize.ToString() != ChatFontSize.Text) Properties.Settings.Default.ChatFontSize = ChatFontSizeOut;
            Properties.Settings.Default.Save();//保存

            this.Close();//关闭窗口
            return;


        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
    }
}
