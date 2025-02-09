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
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Deepseek_CSApp.APIkey);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                Deepseek_CSApp.messages.Add(new { role = "user", content = ChatIn.Text });

                // 构造请求体
                var requestBody = new
                {
                    max_tokens = 2048,
                    model = "deepseek-chat", // 使用的模型
                    messages = Deepseek_CSApp.messages,
                    stream = false // 是否流式传输
                };
                MainContrl.IsEnabled = false;
                ChatOut("我: " + ChatIn.Text);
                ChatIn.Text = "";

                // 将请求体序列化为JSON
                string jsonContent = JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 发送POST请求
                HttpResponseMessage response = await client.PostAsync(Deepseek_CSApp.APIURL, httpContent);
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainContrl.Margin = new Thickness(MainContrl.Margin.Left, ChatShow.ActualHeight, MainContrl.Margin.Right, MainContrl.Margin.Bottom);


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
                } 
                else SendButton_Click(sender, null);

            }
            
            
        }

        private void ChatIn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ChatIn.Text=="") Send.IsEnabled= false;
            else Send.IsEnabled= true;
        }
    }
}