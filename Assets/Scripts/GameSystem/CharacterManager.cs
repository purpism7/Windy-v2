using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Creator;
using Creature;
using Creature.Characters;

namespace GameSystem
{
    public interface ICharacterManager : IManager
    { 
        UniTask<T> CreateAsync<T>(int id, Transform rootTm) where T : Creature.Character;

        void InitializeNonPlayables(NonPlayable[] nonPlayables);
        NonPlayable GetNonPlayable(int id);
    }
    
    public class CharacterManager : Manager, ICharacterManager
    {
        private Dictionary<int, Creature.Character> _characterDic = null;
        private List<NonPlayable> _nonPlyableList = null;
        
        public static Playable Playable { get; private set; } = null;

        async UniTask<GameSystem.IGeneric> GameSystem.IGeneric.InitializeAsync()
        {   
            return this;
        }
        
        void GameSystem.IGeneric.ChainUpdate()
        {
            Playable?.ChainUpdate();

            if (_characterDic != null)
            {
                foreach (var character in _characterDic.Values)
                {
                    if (character == null || !character.IsActivate)
                        continue;

                    character.ChainUpdate();
                }
            }
        }

        void GameSystem.IGeneric.ChainLateUpdate()
        {
            Playable?.ChainLateUpdate();
            
            if (_characterDic != null)
            {
                foreach (var character in _characterDic.Values)
                {
                    if (character == null || !character.IsActivate)
                        continue;

                    character.ChainLateUpdate();
                }
            }
        }
        
        void GameSystem.IGeneric.ChainFixedUpdate()
        {
            Playable?.ChainFixedUpdate();
            
            if (_characterDic != null)
            {
                foreach (var character in _characterDic.Values)
                {
                    if (character == null || !character.IsActivate)
                        continue;

                    character.ChainFixedUpdate();
                }
            }
        }

        public async UniTask CreatePlayableAsync()
        {
            Playable = await CreateAsync<Playable>(1, Manager.Get<IRegion>()?.PlayableRootTm);
            Playable?.Initialize();
        }
        
        #region ICharacterManager
        public async UniTask<T> CreateAsync<T>(int id, Transform rootTm = null) where T : Character
        {
            var character = CharacterCreator<T>.Get
                // .SetKey(GetKey(id))
                .SetId(id)
                .SetRoot(rootTm)
                .Create();

            if (character == null)
                return null;
            
            if (_characterDic == null)
            {
                _characterDic = new();
                _characterDic.Clear();
            }
            
            _characterDic?.TryAdd(character.Id, character);
            
            return character;
        }

        void ICharacterManager.InitializeNonPlayables(NonPlayable[] nonPlayables)
        {
            if (nonPlayables.IsNullOrEmpty())
                return;

            if(_nonPlyableList == null)
            {
                _nonPlyableList = new();
                _nonPlyableList.Clear();
            }

            _nonPlyableList?.AddRange(nonPlayables);

            for(int i = 0; i < _nonPlyableList.Count; ++i)
            {
                _nonPlyableList[i]?.Initialize();
            }
        }

        NonPlayable ICharacterManager.GetNonPlayable(int id)
        {
            for (int i = 0; i < _nonPlyableList.Count; ++i)
            {
                var nonPlayable = _nonPlyableList[i];
                if (nonPlayable == null)
                    continue;

                if (!nonPlayable.IsActivate)
                    continue;

                if (nonPlayable.Id == id)
                    return nonPlayable;
            }

            return null;
        }

        // public async UniTask<T> LoadAsync<T>(int id, Transform rootTm = null) where T : Character
        // {
        //     if (_characterCachedDic == null)
        //     {
        //         _characterCachedDic = new();
        //         _characterCachedDic.Clear();
        //     }
        //     
        //     if (_characterCachedDic.TryGetValue(id, out Character character))
        //     {
        //         if (character != null)
        //             return character as T;
        //     }
        //
        //     return null;
        // }

        #endregion

        private string GetKey(int id)
        {
            switch (id)
            {
                case 1: return $"NonPlayable/Momo.prefab";
            }

            return string.Empty;
        }
    }
}