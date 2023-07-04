using Quartz;

namespace MyaDiscordBot.GameLogic.Services
{
    public class BackupJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            if (!Directory.Exists("Backup"))
            {
                _ = Directory.CreateDirectory("Backup");
            }
            if (!Directory.Exists("Save"))
            {
                _ = Directory.CreateDirectory("Save");
            }
            foreach (string files in Directory.GetFiles("Backup"))
            {
                FileInfo fileInfo = new(files);
                if ((DateTime.Now - fileInfo.CreationTime).TotalHours > 48)
                {
                    fileInfo.Delete();
                }
            }
            foreach (string files in Directory.GetFiles("Save"))
            {
                File.Copy(files, files.Replace("Save", "Backup") + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".backup");
            }
            return Task.CompletedTask;
        }
    }
}
