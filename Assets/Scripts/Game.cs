using System;
using System.Collections;
using System.Collections.Generic;
using Creator;
using UnityEngine;

using Cysharp.Threading.Tasks;

using GameSystem;
using Creature;
using Network;
using Network.Api;

public class Game : MonoBehaviour
{
    private float _loadTime = 0;
    private async void Awake()
    {
        // MainManager.Instance
        Debug.Log("Game Awake");
        
        ApiClient.Create();
        await InfoManager.Instance.InitializeAsync();
        await LoadDataAsync();

        await AddressableManager.Instance.InitializeAsync();
        await AtlasManager.Instance.InitializeAsync();
        await MainManager.Instance.InitializeAsync();
        await UIManager.Instance.InitializeAsync();
        // List<UniTask> taskList = new List<UniTask>();
        // taskList.Clear();
        // taskList.Add(UniTask.Defer(async () => await LoadInfoAsync()));
        // taskList.Add(UniTask.Defer(async () => await LoadDataAsync()));
        // taskList.Add(UniTask.Defer(async () => await MainManager.Instance.InitializeAsync()));
        // await UniTask.WhenAll(taskList);
        
        
        Debug.Log("Load Time = " + _loadTime);
    }

    private void FixedUpdate()
    {
        _loadTime += Time.fixedUnscaledDeltaTime;
    }

    private async UniTask LoadDataAsync()
    {
        var dataContainter = FindAnyObjectByType<DataContainer>();
        if (dataContainter != null)
            await dataContainter.InitializeAsync();
    }
}