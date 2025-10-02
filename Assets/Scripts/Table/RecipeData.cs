using Newtonsoft.Json;
using UnityEngine;

namespace Table
{
    public class RecipeData : Data
    {
        [JsonProperty("quest_group")] public int QuestGroup { get; set; } = 0;
        
        [JsonProperty("quest_step")] public int QuestStep { get; set; } = 0;
        
        [JsonProperty("material_item_ids")] 
        [JsonConverter(typeof(JsonConverter<int[]>))]
        public int[] MaterialItemIds { get; private set; } = null;
        
        [JsonProperty("material_item_counts")] 
        [JsonConverter(typeof(JsonConverter<int[]>))]
        public int[] MaterialItemCounts { get; private set; } = null;

        [JsonProperty("result_item_id")] public int ResultItemId { get; private set; } = 0;
        
        [JsonProperty("result_item_count")] public int ResultItemCount { get; private set; } = 0;
        
        [JsonProperty("is_craft")] public bool IsCraft { get; private set; } = false;
    }
}

