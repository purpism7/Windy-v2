using UnityEngine;
using System.Collections.Generic;

using Newtonsoft.Json;

using Common;

namespace Table
{
    public sealed class QuestData : Data
    {
        [JsonProperty("group")]
        public int Group = 0;
        
        [JsonProperty("step")]
        public int Step = 0;
        
        [JsonProperty("condition_1")]
        public EMissionCondition EMissionCondition1 = EMissionCondition.None;
        
        [JsonProperty("value_1")]
        [JsonConverter(typeof(JsonConverter<int[]>))]
        public int[] Value1 = null;
        
        [JsonProperty("condition_2")]
        public EMissionCondition EMissionCondition2 { get; private set; } = EMissionCondition.None;
        
        [JsonProperty("value_2")]
        [JsonConverter(typeof(JsonConverter<int[]>))]
        public int[] Value2 { get; private set; } = null;
        
        [JsonProperty("npc_id")]
        public int NpcId { get; private set; } = 0;
        
        [JsonProperty("complete_talk_local_ids")]
        [JsonConverter(typeof(JsonConverter<int[]>))]
        public int[] CompleteTalkLocalIds { get; private set; } = null;
        
        [JsonProperty("reward_ids")]
        [JsonConverter(typeof(JsonConverter<int[]>))]
        public int[] RewardIds { get; private set; } = null;
        
        [JsonProperty("recipe_ids")]
        [JsonConverter(typeof(JsonConverter<int[]>))]
        public int[] RecipeIds { get; private set; } = null;

        public List<(EMissionCondition, int[])> ConditionList
        {
            get
            {
                var list = new List<(EMissionCondition, int[])>();
                list.Clear();
                
                list.Add((EMissionCondition1, Value1));
                list.Add((EMissionCondition2, Value2));
        
                return list;
            }
        }
    }
}

