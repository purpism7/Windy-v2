using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Event
{
    public class Item : EventParam
    {
   
    }

    public sealed class AddItem : Item
    {
        public int Id { get; private set; } = 0;
        public int Count { get; private set; } = 0;
        
        public AddItem(int id, int count)
        {
            Id = id;
            Count = count;
        }
    }
    
    public sealed class RemoveItem : Item
    {
        public IReadOnlyDictionary<int, int> ItemDic { get; private set; } = null;

        public RemoveItem(Dictionary<int, int> itemDic)
        {
            ItemDic = itemDic;
        }
    }
}


