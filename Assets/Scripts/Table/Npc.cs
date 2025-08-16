using UnityEngine;

using Newtonsoft.Json;

namespace Table
{
    public class Npc : Data
    {
        [JsonProperty("region")] 
        public int Region { get; private set; } = 0;
    }
}

