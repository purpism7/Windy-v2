using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using GameSystem;

namespace GameSystem
{
    public class MainManager : Common.Singleton<MainManager>
    {
        private List<IGeneric> _iMgrGenericList = null;
        private DayNightCycle _dayNightCycle = null;

        public IWeatherManager IWeatherManager { get; private set; } = null;
        public NavMeshPlus.Components.NavMeshSurface NavMeshSurface { get; private set; } = null;
        public IEnumerable<IGeneric> IMgrGenericList => _iMgrGenericList;

        public override async UniTask InitializeAsync()
        {
            if (_iMgrGenericList == null)
            {
                _iMgrGenericList = new();
                _iMgrGenericList.Clear();
            }

            // 순서 중요.
            await AddIGenericAsync(GetComponent<CameraManager>());
            await AddIGenericAsync(transform.AddOrGetComponent<InputManager>());
            await AddIGenericAsync(transform.AddOrGetComponent<MissionManager>());
            await AddIGenericAsync(transform.AddOrGetComponent<RegionManager>());
            await AddIGenericAsync(transform.AddOrGetComponent<CharacterManager>());
            await AddIGenericAsync(transform.AddOrGetComponent<ObjectManager>());

            IWeatherManager = FindFirstObjectByType<WeatherManager>(FindObjectsInactive.Include);
            await IWeatherManager.InitializeAsync();

            _dayNightCycle = FindFirstObjectByType<DayNightCycle>(FindObjectsInactive.Include);
            NavMeshSurface = FindFirstObjectByType<NavMeshPlus.Components.NavMeshSurface>(FindObjectsInactive.Include);
        }

        private async UniTask AddIGenericAsync<T>(T t) where T : IGeneric
        {
            var iGeneric = await t.InitializeAsync();
            _iMgrGenericList?.Add(iGeneric);
        }

        private void Update()
        {
            _dayNightCycle?.ChainUpdate();

            for (int i = 0; i < _iMgrGenericList?.Count; ++i)
                _iMgrGenericList[i]?.ChainUpdate();
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _iMgrGenericList?.Count; ++i)
                _iMgrGenericList[i]?.ChainLateUpdate();
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _iMgrGenericList?.Count; ++i)
                _iMgrGenericList[i]?.ChainFixedUpdate();
        }
    }
}


