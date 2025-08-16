using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Common;
using Weather;

namespace GameSystem
{
    public interface IWeatherManager : IGeneric
    {
        void Apply(Common.EWeather eWeather);
    }
    
    public class WeatherManager : Manager, IWeatherManager
    {
        [Serializable]
        public class Weather
        {
            [SerializeField] private Common.EWeather eWeather = EWeather.None;
            [SerializeField] private Transform transform = null;
            
            public Common.EWeather EWeather
            {
                get { return eWeather; }
            }
            
            public Transform Transform
            {
                get { return transform; }
            }
        }
        
        [SerializeField] private List<Weather> weatherList = null;
        
        [SerializeField] private Transform dustTm = null;
        
        [Header("Wind")]
        [SerializeField] private GameObject windGameObjPrefab = null;
        [SerializeField] private Transform windRootTm = null;
        
        [Header("Rain")]
        [SerializeField] private GameObject[] raindropGameObjPrefabs = null;
        [SerializeField] private Transform raindropRootTm = null;
        
        public Common.EWeather EWeather { get; private set; } = EWeather.None;

        #region IGeneric
        async UniTask<GameSystem.IGeneric> GameSystem.IGeneric.InitializeAsync()
        {
            CreateWindAsync().Forget();
            CreateRaindropAsync().Forget();
            
            Apply(EWeather.Sunny);
            
            return this;
        }

        void GameSystem.IGeneric.ChainUpdate()
        {
            
        }

        void GameSystem.IGeneric.ChainLateUpdate()
        {
            
        }

        void GameSystem.IGeneric.ChainFixedUpdate()
        {
            
        }
        #endregion
        
        #region Wind
        private async UniTask CreateWindAsync()
        {
            for (int i = 0; i < 4; ++i)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            
                var windGameObj = Instantiate(windGameObjPrefab, windRootTm);
                var wind = windGameObj.GetComponent<Wind>();
                wind?.Initialize();
            }
        }
        #endregion
        
        #region Rain
        private async UniTask CreateRaindropAsync()
        {
            for (int i = 0; i < 3; ++i)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            
                CreateRaindrop(raindropGameObjPrefabs?.FirstOrDefault());
            }
            
            for (int i = 0; i < 3; ++i)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.3f));

                CreateRaindrop(raindropGameObjPrefabs?.LastOrDefault());
            }
        }

        private void CreateRaindrop(GameObject gameObjPrefab)
        {
            var raindropGamObj = Instantiate(gameObjPrefab, raindropRootTm);
            var rainDrop = raindropGamObj.GetComponent<Raindrop>();
            rainDrop?.Initialize();
        }
        #endregion
        
        #region IWeatherManager

        public void Apply(EWeather eWeather)
        {
            if (EWeather == eWeather)
                return;

            if (weatherList.IsNullOrEmpty())
                return;

            for (int i = 0; i < weatherList.Count; ++i)
            {
                var weather = weatherList[i];
                if (weather == null)
                    continue;

                Extensions.SetActive(weather.Transform, weather.EWeather == eWeather);
            }
            
            Extensions.SetActive(raindropRootTm, false);
            Extensions.SetActive(windRootTm, false);
            Extensions.SetActive(dustTm, false);
            
            switch (eWeather)
            {
                case EWeather.Rainy:
                {
                    Extensions.SetActive(raindropRootTm, true);
                    break;
                }
                
                case EWeather.Sunny:
                {
                    Extensions.SetActive(windRootTm, true);
                    Extensions.SetActive(dustTm, true);
                    break;
                }
            }
            
            EWeather = eWeather;
        }
        #endregion
    }
}

