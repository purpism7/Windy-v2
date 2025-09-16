using System;
using UnityEngine;

namespace Common
{
    public enum EJsonFile
    {
        None,
    
        User,
        Inventory,
        Quest,
        Recipe,
        Puzzle,
    }

    public enum EMissionCondition
    {
        None,
    
        TalkNpc,
        BringItem,
        PathFindPuzzle,
    }

    public enum EItem
    {
        None,
    
        Material,
        Consumable,
        Equipped,
        Recipe,
    }

    [System.Flags]
    public enum EItemInteraction
    {
        None = 0,
    
        Hammer = 1 << 0,
        Axe = 1 << 1,
        Pickaxe = 1 << 2,
    }

    public enum EAtlasKey
    {
        None,
    
        UIItems,
    }

    public enum EDirection
    {
        None,
            
        Up,
        Down,
        Left,
        Right,
    }

    public enum EInteraction
    {
        None,
    
        Talk,
        QuestClear,
    }

    public enum EWeather
    {
        None,
    
        Sunny,
        Rainy,
    }
    
    [Flags]
    public enum EInputLock
    {
        None = 0,
            
        Key = 1 << 0,
        Axis = 1 << 1,
        
        All = Key | Axis,
    }
}

