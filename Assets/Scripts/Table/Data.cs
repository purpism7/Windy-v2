using Newtonsoft.Json;
using UnityEngine;

namespace Table
{
    public class Data
    {
        [JsonProperty("id")]
        public int Id { get; private set; } = 0;
    }
}

