using MyaDiscordBot.Models.MyaWebsite;
using Newtonsoft.Json;
using Quartz;

namespace MyaDiscordBot.GameLogic.Services
{
    internal class MYASiteJob : IJob
    {
        private readonly HttpClient _httpClient;
        public MYASiteJob()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.mya-hkvtuber.com/api/mya/")
            };
        }
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]: Updating MYA video list...");
            HttpResponseMessage r = await _httpClient.GetAsync("getfuturevid");
            YTData json = JsonConvert.DeserializeObject<YTData>(await r.Content.ReadAsStringAsync());
            Data.Instance.Youtube = json;
        }
    }
}
