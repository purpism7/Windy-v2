using System;
using UnityEngine;

using Newtonsoft.Json;

namespace Table
{
    public sealed class ObjectData : Data
    {
        [JsonProperty("drop_item_ids")]
        [JsonConverter(typeof(JsonConverter<int[]>))]
        public int[] DropItemIds { get; private set; } = null;
    }
}