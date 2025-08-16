using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Creator;

namespace GameSystem
{
    public interface IObjectManager : IManager
    {
        Creature.IObject CreateItemObject(int itemId, Transform rootTm, Vector3 pos);
    }
    
    public class ObjectManager : Manager, IObjectManager
    {
        private List<Creature.IObject> _itemObjectPoolingList = null; 
        
        #region IGeneric
        async UniTask<GameSystem.IGeneric> GameSystem.IGeneric.InitializeAsync()
        {
            
            return this;
        }
        
        void GameSystem.IGeneric.ChainUpdate()
        {
            
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

        #region IObjectManager
        Creature.IObject IObjectManager.CreateItemObject(int itemId, Transform rootTm, Vector3 position)
        {
            if (_itemObjectPoolingList == null)
            {
                _itemObjectPoolingList = new();
                _itemObjectPoolingList.Clear();
            }

            Creature.IObject iObject = null;
            for (int i = 0; i < _itemObjectPoolingList.Count; ++i)
            {
                var itemObject = _itemObjectPoolingList[i];
                if(itemObject == null)
                    continue;
                
                if(itemObject.IsActivate)
                    continue;

                if (itemObject.Id == itemId)
                {
                    iObject = itemObject;
                    break;
                }
            }

            if (iObject == null)
            {
                iObject = ObjectCreator.Get?
                    .SetId(itemId)
                    .SetRoot(rootTm)
                    .SetPosition(position)
                    .Create();
                
                _itemObjectPoolingList?.Add(iObject);
            }
            
            iObject?.SetPosition(position);
            iObject?.SortingOrder(-iObject.Transform.position.y);
            iObject?.Activate();

            return iObject;
        }
        #endregion
    }
}

