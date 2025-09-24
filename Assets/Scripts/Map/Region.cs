using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;
using Creature;
using UI;

namespace Map
{
    public class Region : Common.Component
    {
        [SerializeField] private int id = 0;
        [SerializeField] private Transform itemObjectRootTm = null;
        [SerializeField] private Transform placedObjectRootTm = null;
        [SerializeField] private Transform npcRootTm = null;
        [SerializeField] private Transform playableRootTm = null;
        
        public Transform PlayableRootTm => playableRootTm;
        public Transform ItemObjectRootTm => itemObjectRootTm;

        public override void Initialize()
        {
            base.Initialize();

            var nonPlayables = npcRootTm.GetComponentsInChildren<Creature.Characters.NonPlayable>(true);
            Manager.Get<ICharacterManager>()?.InitializeNonPlayables(nonPlayables);
        }

        public override void Activate()
        {
        
        }

        public override void ChainUpdate()
        {
        
        }

        // private async UniTask CreateAnimalAsync()
        // {
        //     var iCharacterMgr = Manager.Get<ICharacter>();
        //     if (iCharacterMgr == null)
        //         return;
        //     
        //     var animal = await iCharacterMgr.AddAsync<NonPlayable>(1, npcRootTm);
        //     animal?.Activate();
        // }
    }
}

