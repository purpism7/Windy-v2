using UnityEngine;

namespace GameSystem.Event
{
    public class Recipe : EventParam
    {
        public int Id { get; private set; } = 0;

        protected Recipe()
        {
        }
        
        protected Recipe(int id)
        {
            Id = id;
        }
    }

    public class AddRecipe : Recipe
    {
        public AddRecipe()
        {
            
        }
    }
}

