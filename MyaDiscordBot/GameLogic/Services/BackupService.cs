using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Services
{
    public class BackupJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            if (!Directory.Exists("Backup"))
            {
                Directory.CreateDirectory("Backup");
            }
            if (!Directory.Exists("Save"))
            {
                Directory.CreateDirectory("Save");
            }
            foreach(var files in Directory.GetFiles("Backup"))
            {
                FileInfo fileInfo = new FileInfo(files);
                if((DateTime.Now - fileInfo.CreationTime).TotalHours > 6)
                {
                    fileInfo.Delete();
                }
            }
            foreach(var files in Directory.GetFiles("Save"))
            {
                File.Copy(files, files.Replace("Save", "Backup") + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".backup");
            }
            return Task.CompletedTask;
        }
    }
}
