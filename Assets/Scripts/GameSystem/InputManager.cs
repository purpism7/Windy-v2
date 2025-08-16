using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

using Cysharp.Threading.Tasks;

using Common;
using Creator;
using Creature;

namespace GameSystem
{
    public interface IInput : GameSystem.IManager
    {
        void AddListener(InputManager.IListener iListener);
        void RemoveListener(InputManager.IListener iListener);
        bool IsStopped { get; }
        // void Lock(ELock eLock);
    }

    public interface IInputLocker : GameSystem.IManager
    {
        void Lock(EInputLock eLock);
    }
    
    public class InputManager : MonoBehaviour, IInput, IInputLocker
    {
        public interface IListener
        {
            
        }
        
        public interface IAxisListener : IListener
        {
            void OnAxisChanged(float horizontal, float vertical);
        }
        
        public interface IKeyListener : IListener
        {
            void OnKey(KeyCode keyCode);
            void OnKeyDown(KeyCode keyCode);
        }

        public interface IMouseListener : IListener
        {
            void OnMouseButtonDown();
        }

        private HashSet<IListener> _iListenerHashSet = null;
        private EInputLock _eInputLock = EInputLock.None;
        
        async UniTask<GameSystem.IGeneric> GameSystem.IGeneric.InitializeAsync()
        {
            return this;
        }
        
        void GameSystem.IGeneric.ChainUpdate()
        {
            // if (Input.GetMouseButtonDown(0))
            // {
            //     NotifyHandleMouseButtonDown();
            //     return;
            // }

            // if (!EventSystem.current.IsPointerOverGameObject())
            // {
            //     
            // }

            if ((_eInputLock & EInputLock.Key) == 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    NotifyHandleKeyDown(KeyCode.Space);
                    return;
                }
           
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    NotifyHandleKeyDown(KeyCode.Tab);
                    return;
                }
            
                if (Input.GetKey(KeyCode.Escape))
                {
                    NotifyHandleKeyDown(KeyCode.Escape);
                    return;
                }
            
                if (Input.GetKey(KeyCode.Space))
                {
                    NotifyHandleKey(KeyCode.Space);
                    return;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.P))
            {
                //UICreator<UI.Puzzle.PathFindPuzzleView, UI.Puzzle.PathFindPuzzleView.Param>
                //    .Get?.SetRootTm(UIManager.Instance?.PanelUIRootRectTm)?
                //    .SetParam(new UI.Puzzle.PathFindPuzzleView.Param(2))
                //    .SetInitializeSize(true)
                //    .CreateAsync();

                ////puzzlePanel?.Activate(new PathFindPuzzleView.Param(1));
                //return;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                CutscenePlayer.Play(0, 
                    () => 
                    {
                        Debug.Log("Finished Cutscene");
                    });
            }

            if ((_eInputLock & EInputLock.Axis) == 0)
            {
                var horizontal = Input.GetAxis("Horizontal");
                var vertical = Input.GetAxis("Vertical");
            
                NotifyAxisChanged(horizontal, vertical);
            }
        }

        void GameSystem.IGeneric.ChainLateUpdate()
        {
            return;
        }

        void IGeneric.ChainFixedUpdate()
        {
            return;
        }
        
        private void NotifyHandleMouseButtonDown()
        {
            if (_iListenerHashSet.IsNullOrEmpty())
                return;
            
            var iListenerList = _iListenerHashSet.ToList();
            for (int i = 0; i < iListenerList.Count; ++i)
            {
                (iListenerList[i] as IMouseListener)?.OnMouseButtonDown();
            }
        }
        
        private void NotifyHandleKey(KeyCode keyCode)
        {
            if (_iListenerHashSet.IsNullOrEmpty())
                return;
            
            var iListenerList = _iListenerHashSet.ToList();
            for (int i = 0; i < iListenerList.Count; ++i)
            {
                (iListenerList[i] as IKeyListener)?.OnKey(keyCode);
            }
        }

        private void NotifyHandleKeyDown(KeyCode keyCode)
        {
            if (_iListenerHashSet.IsNullOrEmpty())
                return;
            
            var iListenerList = _iListenerHashSet.ToList();
            for (int i = 0; i < iListenerList.Count; ++i)
            {
                (iListenerList[i] as IKeyListener)?.OnKeyDown(keyCode);
            }
        }

        private void NotifyAxisChanged(float horizontal, float vertical)
        {
            if (_iListenerHashSet.IsNullOrEmpty())
                return;
            
            var iListenerList = _iListenerHashSet.ToList();
            for (int i = 0; i < iListenerList.Count; ++i)
            {
                (iListenerList[i] as IAxisListener)?.OnAxisChanged(horizontal, vertical);
            }
        }
        
        void IInput.AddListener(IListener iListener)
        {
            if (_iListenerHashSet == null)
            {
                _iListenerHashSet = new();
                _iListenerHashSet.Clear();
            }

            _iListenerHashSet?.Add(iListener);
        }

        void IInput.RemoveListener(InputManager.IListener iListener)
        {
            if (_iListenerHashSet.IsNullOrEmpty())
                return;

            _iListenerHashSet.Remove(iListener);
        }

        bool IInput.IsStopped
        {
            get
            {
                return Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0;
            }
        }

        void IInputLocker.Lock(EInputLock eInputLock)
        {
            if (eInputLock == EInputLock.None)
                _eInputLock = eInputLock;
            else
                _eInputLock |= eInputLock;
        }
    }
}

