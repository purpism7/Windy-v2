using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

using Info;
using Network;
using Common;
using Network.Api;

public class InfoManager : Singleton<InfoManager>
{
    private static Dictionary<System.Type, Record> _recordDic = null;

    protected override void Initialize()
    {
        DontDestroyOnLoad(this);
    }

    public override async UniTask InitializeAsync()
    {
        if (_recordDic == null)
        {
            _recordDic = new();
            _recordDic.Clear();
        }
        
        await LoadLocalInfosAsync();

        Get<Inventory>()?.Initialize();
    }

    private async UniTask LoadLocalInfosAsync()
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        var filePaths = ApiClient.Instance?.LocalFilePaths;
        for (int i = 0; i < filePaths?.Length; ++i)
        {
            var filePath = filePaths[i];
            var fileName = Path.GetFileName(filePath);

            fileName = Path.GetFileNameWithoutExtension(fileName);
            var type = System.Type.GetType($"{nameof(Info)}.{fileName}");
            if(type == null)
                continue;
            
            string jsonString = File.ReadAllText(filePath);
            var record = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, type, settings);
            
            _recordDic?.TryAdd(type, record as Record);
            Debug.Log(jsonString);
        }
    }

    public static T Get<T>() where T : Record, new()
    {
        Record record = null;

        if (_recordDic.TryGetValue(typeof(T), out record))
            return record as T;
       
        record = new T();
        _recordDic?.TryAdd(typeof(T), record);

        return record as T;
    }
    
    #region Inventory
    public Info.Inventory AddItem(int id, int count)
    {
        var inventory = Get<Inventory>();
        if (inventory == null)
            inventory = new();
        
        return inventory.AddItem(id, count);
    }
    
    public Info.Inventory RemoveItem(Dictionary<int, int> itemDic)
    {
        if (itemDic.IsNullOrEmpty())
            return null;
        
        var inventory = Get<Inventory>();
        if (inventory == null)
            return null;

        return inventory.RemoveItem(itemDic);
    }

    public int GetItemCount(int id)
    {
        var inventory = Get<Inventory>();
        if (inventory == null)
            return 0;

        return inventory.GetItemCount(id);
    }

    public bool HasItem(int itemId)
    {
        return GetItemCount(itemId) > 0;
    }
    
    public bool HasItem(EItemInteraction eItemInteraction)
    {
        var inventory = Get<Inventory>();
        if (inventory == null)
            return false;

        return inventory.HasItem(eItemInteraction);
    }
    
    #endregion
    
    #region Quest
    public Info.Quest SaveQuest(int questGroup, int questStep)
    {
        var quest = Get<Info.Quest>();
        if (quest == null)
            return null;
        
        quest.Set(questGroup, questStep);
        
        return quest;
    }
    #endregion
}
