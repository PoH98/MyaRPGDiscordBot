using MyaDiscordBot.Models.MyaWebsite;
using Newtonsoft.Json;
using Quartz;

namespace MyaDiscordBot.GameLogic.Services
{
    internal class MYASiteJob : IJob
    {
        private HttpClient _httpClient;
        public MYASiteJob()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://www.mya-hkvtuber.com/api/mya/");
        }
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]: Updating MYA video list...");
            var r = await _httpClient.GetAsync("getfuturevid");
            var json = JsonConvert.DeserializeObject<YTData>(await r.Content.ReadAsStringAsync());
            Data.Instance.Youtube = json;
        }
    }
}
