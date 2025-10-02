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
        TalkNpcs,
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

    public enum ETimeOfDay
    {
        None,

        Day, 
        Night,
    }
    
    [Flags]
    public enum EInputLock
    {
        None = 0,
            
        Key = 1 << 0,
        Axis = 1 << 1,
        
        All = Key | Axis,
    }

    #region Visibility
    public enum VisibilityCondition
    {
        None,

        Quest,
        Weather,
        TimeOfDay,
    }

    public enum VisibilityPhase
    {
        None, 

        Before,
        During,
        After,
    }

    public enum VisibilityType
    {
        None,

        Visible,
        Invisible,
    }
    #endregion
}

