
using UnityEngine;

using Newtonsoft.Json;

namespace Table
{
    public sealed class ItemData : Data
    {
        [JsonProperty("type")]
        public Common.EItem EItem { get; private set; } = Common.EItem.None;
        
        [JsonProperty("local_key")]
        public string LocalKey { get; private set; } = string.Empty;
        
        [JsonProperty("image_name")]
        public string ImageName { get; private set; } = string.Empty;

        [JsonProperty("interaction_type")]
        public Common.EItemInteraction EItemInteraction { get; private set; } = Common.EItemInteraction.None;
    }
}

