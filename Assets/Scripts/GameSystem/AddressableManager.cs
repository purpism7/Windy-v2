using Creature;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = Creature.Object;

namespace GameSystem
{
    public class AddressableManager : Common.Singleton<AddressableManager>
    {
        private Dictionary<string, string> _cachedKeyDic = null;
        private Dictionary<int, Creature.Object> _objectDic = null;
        private Dictionary<int, Creature.Character> _characterDic = null;

        protected override void Initialize()
        {
            _cachedKeyDic = new();
            _cachedKeyDic?.Clear();

            DontDestroyOnLoad(this);
        }

        public new async UniTask InitializeAsync()
        {
            //await LoadResourceLocationsAsync("Cutscene");

            await LoadCharacterAsync();
            await LoadObjectAsync(1);
        }

        private async UniTask LoadResourceLocationsAsync(string label)
        {
            var locationsHandle = Addressables.LoadResourceLocationsAsync(label);
            await locationsHandle.Task;

            if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to load resource locations.");
                return;
            }

            foreach (var location in locationsHandle.Result)
            {
                if (location == null)
                    continue;

                if (location.ResourceType != typeof(GameObject))
                    continue;

                string key = location.PrimaryKey;
                string lastSegment = key.Split('/').Last();

                _cachedKeyDic?.TryAdd(lastSegment, key);
            }
        }

        private async UniTask LoadCharacterAsync()
        {
            if (_characterDic == null)
            {
                _characterDic = new();
                _characterDic.Clear();
            }
            
            await LoadAssetAsync<GameObject>(nameof(Character),
                (result) =>
                {
                    if (result)
                    {
                        var character = result.GetComponent<Character>();
                        if (character == null)
                            return;
                        
                        // Debug.Log(character.name);

                        _characterDic?.TryAdd(character.Id, character);
                        // _componentDic?.TryAdd(component.GetType(), component);
                    }
                });
        }
        
        private async UniTask LoadObjectAsync(int regionId)
        {
            if (_objectDic == null)
            {
                _objectDic = new();
                _objectDic.Clear();
            }

            var labelKey = $"{nameof(Object)}_{regionId}";
            await LoadAssetAsync<GameObject>(labelKey,
                (result) =>
                {
                    if (result)
                    {
                        var obj = result.GetComponent<Object>();
                        if (obj == null)
                            return;
                        
                        // Debug.Log(obj.name);

                        _objectDic?.TryAdd(obj.Id, obj);
                        // _componentDic?.TryAdd(component.GetType(), component);
                    }
                });
        }

        public T LoadCharacter<T>(int id) where T : Character
        {
            if (_characterDic == null)
                return null;

            if (_characterDic.TryGetValue(id, out Character character))
                return character as T;

            return null;
        }

        public T LoadObject<T>(int id) where T : Object
        {
            if (_objectDic == null)
                return null;

            if (_objectDic.TryGetValue(id, out Object obj))
                return obj as T;

            return null;
        }
        
        public async UniTask LoadAssetAsync<T>(string labelKey, System.Action<T> action)
        {
            var locationAsync = await Addressables.LoadResourceLocationsAsync(labelKey);

            List<UniTask> loadTaskList = new List<UniTask>();
            loadTaskList.Clear();

            foreach (var location in locationAsync)
            {
                if (location.ResourceType != typeof(T))
                    continue;

                var handle = Addressables.LoadAssetAsync<T>(location);
                // 에셋 로드 후 콜백 실행
                var task = handle.ToUniTask().ContinueWith(result =>
                {
                    if (result != null)
                        action?.Invoke(result);
                });

                loadTaskList.Add(task);
            }

            // 모든 로드가 끝날 때까지 대기
            await UniTask.WhenAll(loadTaskList);

            Debug.Log($"✅ {labelKey} 에셋 로드 완료");
        }

        public GameObject LoadAssetInstantiateImmediately(string key, Transform parent = null)
        {
           string resKey = string.Empty;
            _cachedKeyDic?.TryGetValue(key, out resKey);

            try
            {
                return Addressables.InstantiateAsync(resKey, parent).WaitForCompletion();
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadAssetInstantiateImmediately Error: {e}, {resKey}");
                return null;
            }
        }
    }
}