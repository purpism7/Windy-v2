using UnityEngine;
using System.Collections.Generic;

using Newtonsoft.Json;
using NUnit.Framework;

namespace Table
{
    public class TalkData : Data
    {
        [JsonProperty("npc_id")] 
        public int NpcId = 0;
       
        [JsonProperty("quest_group")] 
        public int QuestGroup = 0;
       
        [JsonProperty("quest_step")] 
        public int QuestStep = 0;
        
        [JsonProperty("talk_local_ids")]
        [JsonConverter(typeof(JsonConverter<int[]>))]
        public int[] TalkLocalIds = null;

        // [Jsonex]
        // public List<int> TalkLocalIdList = null;
    }
}

