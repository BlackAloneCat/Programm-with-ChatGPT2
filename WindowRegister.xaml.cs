using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;
using OpenAI_API.Chat;
using OpenAI_API;

namespace Programm_with_ChatGPT
{
    /// <summary>
    /// Interaction logic for WindowRegister.xaml
    /// </summary>
    public partial class WindowRegister : Window
    {
        public WindowRegister()
        {
            InitializeComponent();
            BeginInit();
        }
        public static string first_quation = "Выпиши мне текст из этого файла";

        private void Options_Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();
            window.Show();
        }

        public async Task RequestAsync(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();

            OpenAIAPI api = new OpenAIAPI(window1.IpKey.Text);
            var chat = api.Chat.CreateConversation();

            /// give instruction as System
            chat.AppendSystemMessage(Requ.Text);

            // and get the response
            string response = await chat.GetResponseFromChatbotAsync();
            Answer.Text = response; // "Yes"

            // the entire chat history is available in chat.Messages
            foreach (ChatMessage msg in chat.Messages)
            {
                Console.WriteLine($"{msg.Role}: {msg.Content}");
            }
        }

        public async Task BeginInit(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();

            string apiKey = window1.IpKey.Text;
            string apiUrl = "https://api.openai.com/v1/engines/text-davinci-003/completions"; // Замените на актуальный URL

            string prompt = first_quation;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    prompt = prompt,
                    max_tokens = Convert.ToInt32(window1.Tokens.Text)
                };

                var multipartContent = new MultipartFormDataContent();

                // Добавление JSON-данных в запрос
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                multipartContent.Add(content, "json_data");

                // Загрузка файла в запрос
                byte[] fileBytes = File.ReadAllBytes("путь_к_вашему_файлу.txt");
                var fileContent = new ByteArrayContent(fileBytes);
                multipartContent.Add(fileContent, "file", "название_вашего_файла.txt");

                using (HttpResponseMessage response = await client.PostAsync(apiUrl, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                    else
                    {
                        Errors.Text = $"Ошибка при запросе: {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
            }
        }
    }
}
