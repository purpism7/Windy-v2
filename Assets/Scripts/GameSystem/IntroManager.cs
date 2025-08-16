using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace GameSystem
{
    public class IntroManager : MonoBehaviour
    {
        private Image[] _storyImages = null;
        private float _distance = 20f;
        private float _appearDistance = 500f;
        private int _index = 0;
        private Tween _tween = null;
        private bool _completeLoadGameScene = false;
        
        private void Awake()
        {
            _storyImages = GetComponentsInChildren<Image>();
            foreach (var storyImage in _storyImages)
            {
                storyImage.DOFade(0,0);
            }
        }

        void Start()
        {
            _index = 0;
            _completeLoadGameScene = false;

            StartStory();
        }

        private void StartStory()
        {
            if (_storyImages.IsNullOrEmpty())
            {
                End();
                return;
            }

            if (_storyImages.Length <= _index)
            {
                End();
                return;
            }

            var storyImage = _storyImages[_index];
            if (storyImage == null)
            {
                End();
                return;
            }

            var rectTm = storyImage.GetComponent<RectTransform>();
            rectTm.anchoredPosition = new Vector2(0, -300f);
            
            var localPos = storyImage.transform.localPosition;
            localPos.y -= _distance * 0.5f;
            storyImage.transform.localPosition = localPos;
            
            Sequence appearSequence = DOTween.Sequence()
                .Append(rectTm.DOAnchorPosY(0, 1f).SetEase(Ease.OutCubic))
                .Join(storyImage.DOFade(1f, 1f).SetEase(Ease.OutCubic));

            // 등장 후 둥둥 떠다니는 애니메이션 시작
            appearSequence.OnComplete(() =>
            {
                _tween = rectTm.DOAnchorPosY(rectTm.anchoredPosition.y + _distance, 1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            });

            DOVirtual.DelayedCall(6f, 
                () => {
                    
                _tween.Kill();
                storyImage.transform.DOLocalMoveY(0, 0.1f);
                storyImage.DOFade(0, 0.5f);
                
                ++_index;
                
                StartStory();
                // 또는 floatTween.Pause();
            });
        }

        private void End()
        {
            LoadGameScene();
            
            DOVirtual.DelayedCall(1.5f, 
                () =>
                {
                    if (_completeLoadGameScene)
                        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                });
        }

        private void LoadGameScene()
        {
            var asyncOperation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            if(asyncOperation != null)
                asyncOperation.completed += CompleteLoadScene;
        }

        private void CompleteLoadScene(AsyncOperation operation)
        {
            _completeLoadGameScene = true;
        }
    }
}

