using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Map;

namespace GameSystem
{
    public interface IRegion : IManager
    {
        Transform PlayableRootTm { get; }
        Transform ItemObjectRootTm { get; }
    }
    
    public class RegionManager : Manager, IRegion
    {
        [SerializeField] private Region currRegion = null;

        private List<Region> _regionList = null;

        public Transform PlayableRootTm => currRegion?.PlayableRootTm;
        public Transform ItemObjectRootTm => currRegion?.ItemObjectRootTm;

        #region IGeneric
        async UniTask<GameSystem.IGeneric> GameSystem.IGeneric.InitializeAsync()
        {
            currRegion?.Initialize();
            
            return this;
        }
        
        void GameSystem.IGeneric.ChainUpdate()
        {
            currRegion?.ChainUpdate();
        }

        void GameSystem.IGeneric.ChainLateUpdate()
        {
            return;
        }
        
        void GameSystem.IGeneric.ChainFixedUpdate()
        {
            return;
        }
        #endregion
    }
}


