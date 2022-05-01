﻿using Newtonsoft.Json;

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
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<Resource>(JsonConvert.SerializeObject(this), deserializeSettings);
        }
    }

    public class HoldedResource : Resource
    {
        public HoldedResource(Resource resource)
        {
            var r = (Resource)resource.Clone();
            Id = r.Id;
            Name = r.Name;
            DropRate = r.DropRate;
        }
        public int Amount { get; set; }

    }
}
