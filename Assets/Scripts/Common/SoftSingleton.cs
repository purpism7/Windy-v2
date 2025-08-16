using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cysharp.Threading.Tasks;

namespace Common
{
    public abstract class SoftSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance = null;

        protected static void Create()
        {
            if (!Application.isPlaying)
                return;

            var obj = FindAnyObjectByType<T>();
            if (obj == null)
            {
                var gameObj = new GameObject(typeof(T).Name);
                _instance = gameObj.AddComponent<T>();
            }
            else
                _instance = obj;
        }

        public static bool Validate()
        {
            return _instance != null;
        }

        protected async void Awake()
        {
            if (_instance == null)
                Create();

            var instance = _instance.GetComponent<SoftSingleton<T>>();
            await instance.InitializeAsync();

            // await _instance.GetComponent<Singleton<T>>().InitializeAsync();
        }

        protected virtual UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}
