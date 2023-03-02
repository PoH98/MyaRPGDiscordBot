using Newtonsoft.Json;

namespace MyaDiscordBot.Models
{
    public class Resources : List<Resource>
    {

    }
    public class Resource : ICloneable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double DropRate { get; set; }
        public object Clone()
        {
            JsonSerializerSettings deserializeSettings = new() { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<Resource>(JsonConvert.SerializeObject(this), deserializeSettings);
        }
    }

    public class HoldedResource : Resource
    {
        public HoldedResource()
        {

        }
        public HoldedResource(Resource resource)
        {
            Resource r = (Resource)resource.Clone();
            Id = r.Id;
            Name = r.Name;
            DropRate = r.DropRate;
            Amount = 1;
        }
        public int Amount { get; set; }

    }
}
