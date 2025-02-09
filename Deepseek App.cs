using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Deepseek_Desktop_App
{
    internal class Deepseek_CSApp
    {
        public static string APIkey = "sk-f87563eaf187411dba46f1488fb3fcef";
        public static string APIURL = "https://api.deepseek.com/v1/chat/completions";


        public static List<object> messages = new List<object>
    {
        new { role = "system", content = "你是一个人工智能，在一个由梁清华制作，叫做Deepseek App的Window桌面软件和用户对话。必须注意由于程序的原因，你只无论如何都只输出普通文本，如果要制表，请用换行。" }
    };
    }
}
