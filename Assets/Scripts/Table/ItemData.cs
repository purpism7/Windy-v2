
using UnityEngine;

using Newtonsoft.Json;

namespace Table
{
    public sealed class ItemData : Data
    {
        [JsonProperty("type")]
        public Common.EItem EItem = Common.EItem.None;
        
        [JsonProperty("local_key")]
        public string LocalKey = string.Empty;
        
        [JsonProperty("image_name")]
        public string ImageName = string.Empty;

        [JsonProperty("interaction_type")]
        public Common.EItemInteraction EItemInteraction { get; private set; } = Common.EItemInteraction.None;
    }
}

