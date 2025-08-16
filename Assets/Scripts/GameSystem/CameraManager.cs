using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using DG.Tweening;

using Creature.Characters;
using Cysharp.Threading.Tasks;
using Vector3 = UnityEngine.Vector3;

namespace GameSystem
{
    public interface ICameraManager : GameSystem.IManager
    {
        Camera MainCamera { get; }

        float ReturnDistance { get; }
        //bool IsReturn { get; }
        Vector3 Center { get; }
        float Width { get; }
        float Height { get; }

        void SetPlayable(Playable playable);

        void ActivateVirtualCamera();
        void DeactivateVirtualCamera();
        void SetUpdateMethod(CinemachineBrain.UpdateMethod updateMethod);
    }

    public class CameraManager : MonoBehaviour, ICameraManager
    {
        [SerializeField]
        [Range(0.1f, 5f)]
        private float zoomInOutDuration = 1f;

        [SerializeField]
        private Camera mainCamera = null;
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera = null;
        //private const float DefaultZPos = -200f;

        private Playable _playable  = null;
        //private bool _isMove = false;
        #region Drag
        private const float DirectionForceReduceRate = 0.935f; // 감속비율
        private const float DirectionForceMin = 0.001f; // 설정치 이하일 경우 움직임을 멈춤

        // 변수 : 이동 관련
        private Vector3 _startPosition;  // 입력 시작 위치를 기억
        private Vector3 _directionForce; // 조작을 멈췄을때 서서히 감속하면서 이동 시키기 위한 변수
        #endregion

        // private Creature.Hero _fieldHero = null;
        
        //private float _returnTime = 0;
        
        public Camera MainCamera { get { return mainCamera; } }
        public float ReturnDistance { get; private set; } = 0;

        public Vector3 Center
        {
            get
            {
                if (mainCamera == null)
                    return Vector3.zero;
                
                return mainCamera.transform.position + mainCamera.transform.forward;
            }
        }
        
        public float Width
        {
            get
            {
                if (mainCamera == null)
                    return 0;
                
                float height = Height;
                return height * MainCamera.aspect;
            }
        }
        
        public float Height
        {
            get
            {
                if (mainCamera == null)
                    return 0;
                
                return mainCamera.orthographicSize * 2f;
            }
        }
    
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (mainCamera == null)
                return;

            // Gizmos.color = Color.red;
            // Gizmos.DrawWireCube(_center, _mapSize * 2f);
            // Gizmos.DrawWireSphere(Center, 30f);


            var center = Center;
            float height = mainCamera.orthographicSize * 2f;
            float width = height * mainCamera.aspect;
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(center, new Vector3(width, height));
            
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(10f, new Vector3(width, height));
        }
#endif
        async UniTask<GameSystem.IGeneric> GameSystem.IGeneric.InitializeAsync()
        {
            ReturnDistance = 0;
            
            return this;
        }
        
        void GameSystem.IGeneric.ChainUpdate()
        {
            
        }
        
        void IGeneric.ChainLateUpdate()
        {
            if (mainCamera == null)
                return;

            if (virtualCamera == null)
                return;

            ReduceDirectionForce();
            UpdateCameraPosition();
        }

        void IGeneric.ChainFixedUpdate()
        {
            
        }
        
        public void SetPlayable(Playable playable)
        {
            _playable = playable;
        }

        void ICameraManager.ActivateVirtualCamera()
        {
            virtualCamera?.SetActive(true);    
        }
        
        void ICameraManager.DeactivateVirtualCamera()
        {
            virtualCamera?.SetActive(false);    
        }
        
        void ICameraManager.SetUpdateMethod(CinemachineBrain.UpdateMethod updateMethod)
        {
            var cinemachineBrain = mainCamera?.GetComponent<CinemachineBrain>();
            if (cinemachineBrain == null)
                return;

            cinemachineBrain.m_UpdateMethod = updateMethod;
        }
        
        bool CheckDrag(Vector3 startPos, Vector3 currPos)
        {
            return (currPos - startPos).sqrMagnitude >= 0.01f;
        }
        
        private void StartMove(Vector3 startPosition) 
        {
            _startPosition = startPosition;
            _directionForce = Vector3.zero;
        }
        
        private void ReduceDirectionForce()
        {
            // 조작 중일때는 아무것도 안함
            //if (_isMove)
            //    return;
                
            // 감속 수치 적용
            _directionForce *= DirectionForceReduceRate;
            // 작은 수치가 되면 강제로 멈춤
            if (_directionForce.magnitude < DirectionForceMin)
                _directionForce = Vector3.zero;
        }
        
        private void UpdateCameraPosition()
        {
            if (!_playable?.Transform)
                return;

            var currentPos = mainCamera.transform.position;
            var targetPos = _playable.Transform.position;
            targetPos.z = -50f;
            
            mainCamera.transform.position = Vector3.Lerp(currentPos, targetPos, Time.deltaTime);

            ReturnDistance = Vector3.Distance(currentPos, targetPos);
        }

        //private bool CameraReturnToCharacter()
        //{
        //    if (mainCamera == null)
        //        return false;
            
        //    //if (_isMove ||
        //    //    _returnTime < 1f)
        //    //    return false;
        
        //    var navMeshTm = _playable?.NavMeshAgent?.transform;
        //    if (!navMeshTm)
        //        return false;
        
        //    var targetPos = new Vector3(navMeshTm.position.x, navMeshTm.position.y, -50f);
        //    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, Time.deltaTime * 2f);
        
        //    return true;
        //}
        
        #region Zoom In / Out
        // void ICameraManager.ZoomIn(Vector3 targetPos, Action endAction)
        // {
        //     ZoomInAsync(targetPos, endAction).Forget();
        // }
        //
        // private async UniTask ZoomInAsync(Vector3 targetPos, Action endAction)
        // { 
        //     var duration = zoomInOutDuration;
        //     targetPos.z = DefaultZPos;
        //     
        //     DOTween.To(() => virtualCamera.m_Lens.OrthographicSize,
        //         orthographicSize => virtualCamera.m_Lens.OrthographicSize = orthographicSize, 18f, duration);
        //     await DOTween.To(() => mainCamera.transform.position,
        //         position => mainCamera.transform.position = position, targetPos, duration).SetEase(Ease.OutCirc);
        //
        //     await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        //     
        //     endAction?.Invoke();
        // }
        //
        // void ICameraManager.ZoomOut(Action endAction)
        // {
        //     ZoomOutAsync(endAction).Forget();
        // }
        //
        // private async UniTask ZoomOutAsync(Action endAction)
        // { 
        //     var duration = zoomInOutDuration;
        //     
        //     await DOTween.To(() => virtualCamera.m_Lens.OrthographicSize,
        //         orthographicSize => virtualCamera.m_Lens.OrthographicSize = orthographicSize, DefaultOrthographicSize, duration).SetEase(Ease.Linear);
        //
        //     await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        //     
        //     endAction?.Invoke();
        // }
        #endregion
    }
}

