using Newtonsoft.Json;
using UnityEngine;

namespace Table
{
    public class LocalData : Data
    {
        [JsonProperty("key")] public string Key = string.Empty;
        
        [JsonProperty("ko")] public string Ko = string.Empty;
        
        [JsonProperty("en")] public string En = string.Empty;
    }
}

