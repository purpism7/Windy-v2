using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace GameSystem
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CutscenePlayer : Common.SoftSingleton<CutscenePlayer>, Cutscene.IListener
    {
        private PlayableDirector _playableDirector = null;
        private ICutscene[] _iCutscenes = null;
        private System.Action _finishedAction = null;

        public static void Play(int id, System.Action finishedAction)
        {
            Create();

            SetFinishedAction(finishedAction);
            _instance?.Play(id);
        }

        private static void SetFinishedAction(System.Action finishedAction)
        {
            if (_instance != null)
                _instance._finishedAction = finishedAction;
        }

        protected override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();

            Debug.Log("InitializeAsync()");
            if (_playableDirector == null)
            {
                _playableDirector = GetComponent<PlayableDirector>();
                _playableDirector.stopped += OnStopped;
            }

            _iCutscenes = GetComponentsInChildren<Cutscene>();
            if(_iCutscenes != null)
            {
                foreach (var iCutscene in _iCutscenes)
                {
                    iCutscene?.Intialize(this);
                }
            }
        }

        private void Play(int id)
        {
            Debug.Log("Play()");
            if (_playableDirector == null)
            {
                FinishAsync().Forget();
                return;
            }

            var iCameraManager = Manager.Get<ICameraManager>();
            if (iCameraManager == null)
            {
                FinishAsync().Forget();
                return;
            }
            
            Manager.Get<IInputLocker>()?.Lock(Common.EInputLock.All);
            
            // iCameraManager.DeactivateVirtualCamera();
            Extensions.SetActive(UIManager.Instance?.Canvas, false);
            iCameraManager.SetUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate);

            _iCutscenes[id]?.Play(_playableDirector);
        }    

        private async UniTask FinishAsync()
        {
            var iCameraManager = Manager.Get<ICameraManager>();
            if (iCameraManager == null)
                return;

            // iCameraManager.ActivateVirtualCamera();
            
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            await UniTask.NextFrame();

            if (_playableDirector != null)
                _playableDirector.playableAsset = null;

            iCameraManager.SetUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.SmartUpdate);

            await UniTask.WaitWhile(() => iCameraManager.ReturnDistance <= 1f);
            
            Manager.Get<IInputLocker>()?.Lock(Common.EInputLock.None);
            Extensions.SetActive(UIManager.Instance?.Canvas, true);

            _finishedAction?.Invoke();
        }
        
        private void OnStopped(PlayableDirector playableDirector)
        {
            FinishAsync().Forget();
        }

        #region Cutscene.IListener
        void Cutscene.IListener.Finish()
        {
            FinishAsync().Forget();
        }
        #endregion
    }
}

