using Autofac;

namespace MyaDiscordBot
{
    internal class Data
    {
        private static Data _instance;
        public static Data Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Data();
                }
                return _instance;
            }
        }
        public IContainer Container { get; set; }
    }
}
