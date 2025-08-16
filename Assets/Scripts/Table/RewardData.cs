using UnityEngine;

using Newtonsoft.Json;

namespace Table
{
    public class RewardData : Data
    {
        [JsonProperty("item_id")]
        public int ItemId { get; private set; } = 0;
        
        [JsonProperty("item_count")]
        public int ItemCount { get; private set; } = 0;
    }
}

