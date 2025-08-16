using System;
using System.Collections.Generic;
using System.Linq;

using Table;
using Common;

namespace Info
{
    [Serializable]
    public sealed class Inventory : Record
    {
        private List<Item> _itemList = new();

        public override void Initialize()
        {
            base.Initialize();
        }

        public IReadOnlyList<Item> ItemList => _itemList;
        
        public Inventory AddItem(int id, int count)
        {
            for (int i = 0; i < _itemList?.Count; ++i)
            {
                if(_itemList[i] == null)
                    continue;
            
                if (_itemList[i].Id == id)
                {
                    _itemList[i].Count += count;
                    return this;
                }
            }

            var item = new Info.Item
            {
                Id = id,
                Count = count,
            };
        
            _itemList?.Add(item);

            return this;
        }

        public Inventory RemoveItem(Dictionary<int, int> itemDic)
        {
            foreach (var pair in itemDic)
            {
                int itemId = pair.Key;
                int itemCount = pair.Value;
            
                for (int i = 0; i < _itemList?.Count; ++i)
                {
                    if(_itemList[i] == null)
                        continue;
                    
                    if (_itemList[i].Id == itemId)
                    {
                        _itemList[i].Count -= itemCount;
                        if (_itemList[i].Count <= 0)
                            _itemList.RemoveAt(i);
                        
                        break;
                    }
                }
            }

            return this;
        }

        public int GetItemCount(int id)
        {
            if (_itemList.IsNullOrEmpty())
                return 0;
        
            for (int i = 0; i < _itemList.Count; ++i)
            {
                if(_itemList[i] == null)
                    continue;

                if (_itemList[i].Id == id)
                    return _itemList[i].Count;
            }

            return 0;
        }

        public bool HasItem(EItemInteraction eItemInteraction)
        {
            for (int i = 0; i < _itemList?.Count; ++i)
            {
                if(_itemList[i] == null)
                    continue;
                
                var itemData = ItemDataContainer.Instance?.GetData(_itemList[i].Id);
                if(itemData == null)
                    continue;

                if(itemData.EItem != EItem.Equipped)
                    continue;
                
                if (itemData.EItemInteraction == eItemInteraction)
                    return true;
            }

            return false;
        }
    }
}

