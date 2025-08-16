using System;
using Newtonsoft.Json;

namespace Info
{
    [Serializable]
    public sealed class Quest : Record
    { 
        [JsonProperty] public int Group { get; private set; } = 1;
        [JsonProperty] public int Step { get; private set; } = 1;

        public void Set(int group, int step)
        {
            Group = group;
            Step = step;
        }
    }
}

