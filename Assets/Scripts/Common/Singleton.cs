using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cysharp.Threading.Tasks;

namespace Common
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    Create();

                return _instance;
            }
        }

        public static void Create()
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

            var instance = _instance.GetComponent<Singleton<T>>();
            instance?.Initialize();

            // await _instance.GetComponent<Singleton<T>>().InitializeAsync();
        }

        protected virtual void Initialize()
        {
            return;
        }

        public virtual async UniTask InitializeAsync()
        {

        }
    }
}
