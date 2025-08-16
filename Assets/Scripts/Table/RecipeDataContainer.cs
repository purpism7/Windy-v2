using UnityEngine;

namespace Table
{
    public class RecipeDataContainer : Container<RecipeDataContainer, RecipeData>
    {
        public RecipeData GetDataByResultItemId(int resultItemId)
        {
            if (Datas.IsNullOrEmpty())
                return null;

            for (int i = 0; i < Datas.Length; ++i)
            {
                var data = Datas[i];
                if(data == null)
                    continue;

                if (data.ResultItemId == resultItemId)
                    return data;
            }

            return null;
        }

        // Checks if the player has the required ingredients for crafting a specific item.
        public bool HasRequiredIngredients(int resultItemId)
        {
            var data = GetDataByResultItemId(resultItemId);
            if (data == null)
                return false;

            for (int i = 0; i < data.MaterialItemCounts.Length; ++i)
            {
                int materialItemId = data.MaterialItemIds[i];
                if (data.MaterialItemCounts.IsNullOrEmpty() ||
                    data.MaterialItemCounts.Length <= i)
                    continue;

                int itemCount = InfoManager.Instance.GetItemCount(materialItemId);
                if (itemCount < data.MaterialItemCounts[i])
                    return false;
            }

            return true;
        }
        
    }
}

