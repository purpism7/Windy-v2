using Newtonsoft.Json;
using UnityEngine;

namespace Table
{
    public class LocalData : Data
    {
        [JsonProperty("key")] public string Key { get; private set; } = string.Empty;
        
        [JsonProperty("ko")] public string Ko { get; private set; } = string.Empty;
        
        [JsonProperty("en")] public string En { get; private set; } = string.Empty;
    }
}

