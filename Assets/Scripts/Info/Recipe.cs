using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Newtonsoft.Json;

namespace Info
{
    [Serializable]
    public sealed class Recipe : Record
    {   
        private List<int> _recipeIdList = new();

        public IReadOnlyList<int> RecipeIdList => _recipeIdList; 

        public Recipe Add(int[] ids)
        {
            for (int i = 0; i < ids?.Length; ++i)
            {
                int id = ids[i];
                Add(id);
            }
           
            return this;
        }

        public Recipe Add(int id)
        {
            if (_recipeIdList.Contains(id))
                return this;

            _recipeIdList.Add(id);

            return this;
        }

        public Recipe Remove(int id)
        {
            if (_recipeIdList.IsNullOrEmpty())
                return this;

            _recipeIdList.Remove(id);

            return this;
        }
    }
}



