using Cysharp.Threading.Tasks;
using UnityEngine;

using GameSystem.Mission;

namespace GameSystem
{
    public interface IMission : IManager
    {
        public Quest Quest { get; }
    }
    
    public class MissionManager : Manager, IMission
    {
        public Quest Quest { get; private set; } = null;

        async UniTask<GameSystem.IGeneric> GameSystem.IGeneric.InitializeAsync()
        {
            Quest = transform.AddOrGetComponent<Quest>()?.Initialize();
            
            return this;
        }
        // public IGeneric Initialize()
        // {
        //     Quest = transform.AddOrGetComponent<Quest>()?.Initialize();
        //     
        //     // EventHandler<Ev>.Add(1, OnChanged);
        //     // EventHandler<Ev>.Notify(1, new Ev());
        //     
        //     return this;
        // }
        
        void IGeneric.ChainUpdate()
        {
            
        }

        void IGeneric.ChainLateUpdate()
        {
            return;
        }
        
        void IGeneric.ChainFixedUpdate()
        {
            return;
        }
    }
}

