using System;
using UnityEngine;

namespace Info
{
    [Serializable]
    public sealed class Item : Record
    {
        public int Id = 0;
        public int Count = 0;
    }
}

