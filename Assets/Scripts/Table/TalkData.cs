using UnityEngine;

using Newtonsoft.Json;

namespace Table
{
    public class TalkData : Data
    {
        [JsonProperty("npc_id")] 
        public int NpcId { get; private set; } = 0;
       
        [JsonProperty("quest_group")] 
        public int QuestGroup { get; private set; } = 0;
       
        [JsonProperty("quest_step")] 
        public int QuestStep { get; private set; } = 0;
        
        [JsonProperty("talk_local_ids")]
        [JsonConverter(typeof(JsonConverter<int[]>))]
        public int[] TalkLocalIds { get; private set; } = null;
    }
}

