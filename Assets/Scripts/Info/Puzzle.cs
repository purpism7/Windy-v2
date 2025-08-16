using System;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace Info
{
    [Serializable]
    public sealed class Puzzle : Record
    {
        [Serializable]
        public class TransformInfo
        {
            [JsonProperty] public int ItemId { get; private set; } = 1;
            [JsonProperty] public int PositionIndex { get; private set; } = 1;
            [JsonProperty] public int RotationZ { get; private set; } = 0;
        }
        
        [JsonProperty] public int Index { get; private set; } = 0;
        
        private List<TransformInfo> _transformInfoList = new();
    }
}
