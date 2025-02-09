using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using System.Runtime.InteropServices;
using System.IO;

namespace Deepseek_Desktop_App
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public void ChatOut(string AIOut)
        {
            ChatShow.Text += "\n" + AIOut;

        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建HttpClient实例
            using (HttpClient client = new HttpClient())
            {
                // 设置请求头，包括API密钥
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Properties.Settings.Default.APIkey);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                Deepseek_CSApp.messages.Add(new { role = "user", content = ChatIn.Text });

                // 构造请求体
                var requestBody = new
                {
                    max_tokens = Properties.Settings.Default.max_tokens,
                    model = Properties.Settings.Default.model, // 使用的模型
                    messages = Deepseek_CSApp.messages,
                    stream = false, // 是否流式传输
                    temperature = Properties.Settings.Default.temperature,
                };
                MainContrl.IsEnabled = false;
                ChatOut("我: " + ChatIn.Text);
                ChatIn.Text = "";

                // 将请求体序列化为JSON
                string jsonContent = JsonSerializer.Serialize(requestBody);
                Console.WriteLine(jsonContent);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 发送POST请求
                HttpResponseMessage response = await client.PostAsync(Properties.Settings.Default.APIurl, httpContent);
                Console.WriteLine(response.Content);

                // 检查响应是否成功
                if (response.IsSuccessStatusCode)
                {
                    // 读取响应内容
                    string responseJson = await response.Content.ReadAsStringAsync();

                    // 解析响应内容
                    using (JsonDocument doc = JsonDocument.Parse(responseJson))
                    {
                        JsonElement root = doc.RootElement;
                        string AIReply = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                        Console.WriteLine("AI回复: " + AIReply);
                        ChatOut("Deepseek: " + AIReply + "\n");
                        Deepseek_CSApp.messages.Add(new { role = "assistant", content = AIReply });
                    }
                }
                else
                {
                    Console.WriteLine("请求失败: " + response.StatusCode);
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("错误信息: " + errorResponse);
                    ChatOut("请求失败: " + response.StatusCode + "\n");
                    Deepseek_CSApp.messages.Add(new { role = "assistant", content = response.StatusCode });
                }
            }

            
            MainContrl.IsEnabled = true;
            ChatIn.Focus();
            ChatShow.ScrollToEnd();
            return;
        }

        //窗口大小改变事件
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainContrl.Margin = new Thickness(MainContrl.Margin.Left, ChatShow.ActualHeight, MainContrl.Margin.Right, MainContrl.Margin.Bottom);
            SubContrl.Margin = new Thickness(ChatShow.ActualWidth,SubContrl.Margin.Top,SubContrl.Margin.Right,MainContrl.Margin.Bottom);

        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            ChatIn.Text = "";
        }


        //输入框换行
        private void ChatIn_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key==Key.Enter)
            {
                if (System.Windows.Forms.Control.ModifierKeys == Keys.Control) {
                    ChatIn.Text += "\n";
                    ChatIn.Focus();
                    ChatIn.Select(ChatIn.Text.Length, ChatIn.Text.Length);
                    return;
                } 
                if(Send.IsEnabled) SendButton_Click(sender, null);

            }
            
            
        }

        private void ChatIn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ChatIn.Text=="") Send.IsEnabled= false;
            else Send.IsEnabled= true;
        }

        private void OpenSetting_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingWindow();
            window.Activate();
            window.Show();
            window.Owner = this;
            
        }


        private void ChatIn_Loaded(object sender, RoutedEventArgs e)
        {
            ChatShow.FontSize=Properties.Settings.Default.ChatFontSize;
        }
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChatShow.FontSize = Properties.Settings.Default.ChatFontSize;
        }

        private void ChatReset_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBoxResult Result = System.Windows.MessageBox.Show("你是否要重置对话，对话记录将消除（除非你已经保存过）", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning);//弹出提示框
            if (Result == System.Windows.MessageBoxResult.OK) {
                ChatShow.Text = "——————————对话开始——————————";

                Deepseek_CSApp.messages = new System.Collections.Generic.List<object>
                {
                    new { role = "system", content = "你是一个人工智能，在一个由梁清华制作，叫做Deepseek App的Window桌面软件和用户对话。必须注意由于程序的原因，你只无论如何都只输出普通文本，如果要制表，请用换行。" }
                };

            }
            return;//终止进程
        }

        //保存
        private void SaveChat_Click(object sender, RoutedEventArgs e)
        {
            //list转json string
            var ChatRecord = new { Message = Deepseek_CSApp.messages };
            string ChatRecordJson = JsonSerializer.Serialize(ChatRecord);
            
            
            SaveFileDialog ChatSaveFileDialog = new SaveFileDialog()
            {
                Filter = "Files (*.json)|*.json",//json文件
                FileName = "Deepseek 聊天文件"+ DateTime.Now.ToString("yyyyddhhmmss"),
            };

            //获取路径
            DialogResult ChatSaveFileDialogResult = ChatSaveFileDialog.ShowDialog();
            if (ChatSaveFileDialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Console.WriteLine(ChatSaveFileDialog.FileName);

            }
            else
            {
                Console.WriteLine("用户关闭" + ChatSaveFileDialogResult);
                return;
            }
            File.WriteAllText(ChatSaveFileDialog.FileName, ChatRecordJson);
            return;

        }
        
        //导入
        private void InportChat_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBoxResult Result = System.Windows.MessageBox.Show("你是否要导入对话，这将重置当前对话", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning);//弹出提示框
            if (Result == System.Windows.MessageBoxResult.OK) Console.WriteLine("确认");
            else return;




                OpenFileDialog openFileDialog = new OpenFileDialog()//打开路径选择对话框
                {
                    Multiselect = false,//不可选择多个
                    Title = "请选择你的对话",
                    Filter = "Files (*.json)|*.json"//选择txt文件
                };
            DialogResult dialogResult = openFileDialog.ShowDialog();//获取返回结果
            Console.WriteLine(dialogResult);
            if (dialogResult == System.Windows.Forms.DialogResult.OK)//判断返回结果
            {
                Console.WriteLine(openFileDialog.FileName);
            }
            else
            {
                Console.WriteLine("用户关闭" + dialogResult);
                return;
            }

            //读取
            string ChatJson = File.ReadAllText(openFileDialog.FileName);
            JsonDocument doc = JsonDocument.Parse(ChatJson);
            JsonElement root = doc.RootElement;
            
            //读取Message
            JsonElement messageArray = root.GetProperty("Message");
            int ChatCount = messageArray.GetArrayLength();
            Console.WriteLine("发现聊天"+ChatCount);



            //重置当前对话
            string Content;
            ChatShow.Text = "——————————对话开始——————————";//重置对话框

            Deepseek_CSApp.messages = new System.Collections.Generic.List<object> { };
            foreach (JsonElement item in messageArray.EnumerateArray())
            {

                
                if (item.GetProperty("content").ValueKind == JsonValueKind.Number) Content = item.GetProperty("content").GetInt32().ToString();
                else Content = item.GetProperty("content").GetString();
                Deepseek_CSApp.messages.Add(new { role = item.GetProperty("role").GetString(), content = Content });
                
                //还原对话框
                if(item.GetProperty("role").GetString()== "user") ChatOut("我: " + Content);
                if (item.GetProperty("content").ValueKind == JsonValueKind.Number) ChatOut("请求失败: " + Content);
                if (item.GetProperty("role").GetString() == "assistant") ChatOut("Deepseek: " + Content + "\n");
            };
            




        }
    }
}