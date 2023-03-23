using MyaDiscordBot.Models.ChatGPT;
using Newtonsoft.Json;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using System.Text;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IChatGPTService
    {
        Task<string> GetResponse(string message, string userName);
    }
    public class ChatGPTService : IChatGPTService
    {
        private Config Config;
        private OpenAIService ChatGpt;
        public ChatGPTService()
        {
            if (!File.Exists("ChatGPT.json"))
            {
                File.WriteAllText("ChatGPT.json", JsonConvert.SerializeObject(new Config
                {
                    Token = "FILL YOUR API TOKEN HERE"
                }));
            }
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("ChatGPT.json"));
        }

        public async Task<string> GetResponse(string message, string username)
        {
            if (string.IsNullOrEmpty(Config.Token) || Config.Token == "FILL YOUR API TOKEN HERE")
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("ChatGPT.json"));
                if (string.IsNullOrEmpty(Config.Token) || Config.Token == "FILL YOUR API TOKEN HERE")
                {
                    return "米亞無法回復你哦，佢未有時間setup好個settings，請耐心等待下！";
                }
            }
            if(ChatGpt == null)
            {
                ChatGpt = new OpenAIService(new OpenAiOptions()
                {
                    ApiKey = Config.Token
                });
            }
            var response = await ChatGpt.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem("You are a Panda VTuber named as Mya (米亞) from Hong Kong, having a master named as Gummy (甘米) and a maintainance stuff named as 小貓. You are funny and likes to make jokes. You likes to play poops and having a smelly legs. You are one of the member of 幻花茶屋, which having other members named as Rumii (瑠美), Rabbi (拉比), Luna (露娜). Answer your fans like you are in Discord chatting with them with Cantonese."),
                    ChatMessage.FromUser(message)
                },
                Model = "gpt-3.5-turbo",
                MaxTokens = 256,
                Temperature = 1,
                User = username
            });
            StringBuilder sb = new StringBuilder();
            var selectedResult = response.Choices.FirstOrDefault(x => x.FinishReason == "stop")?.Message?.Content;
            if(selectedResult == null)
            {
                selectedResult = response.Choices.FirstOrDefault(x => x.Message.Content.Length > 0)?.Message?.Content;
            }
            if(selectedResult == null)
            {
                selectedResult = response.Choices.First().Message.Content;
            }
            var lines = selectedResult.Split("\n").Where(x => x.Length > 0);
            if (lines.All(x => x == lines.First()))
            {
                sb.Append(lines.First().Replace("米亞:", "\n").Replace("觀眾:", "\n"));
            }
            else
            {
                foreach(var line in lines)
                {
                    if (line.Contains("米亞:"))
                    {
                        sb.AppendLine(line.Replace("米亞:", ""));
                    }
                    if (line.Contains("觀眾:"))
                    {
                        sb.AppendLine(line.Replace("觀眾:", ""));
                    }
                }
            }
            return sb.ToString();
        }
    }
}
