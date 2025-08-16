using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Cysharp.Threading.Tasks;
using Spine.Unity;

using Creature.Action;
using GameSystem;
using Creature.Characters;

namespace Creature 
{
    public enum EState
    {
        None = 0,
        
        GetItem = 1 << 0,
    }

    public interface IState
    {
        void SetEState(EState eState);
        bool HasEState(EState eState);
        bool IsNone { get; }
    }
    
    public abstract class Character : MonoBehaviour, IActor, IState
    {
        #region Inspector

        [SerializeField] private int id = 0;
        [SerializeField] protected Transform rootTm = null;
        [SerializeField] private float heightOffest = 0;

        #endregion

        protected MeshRenderer _meshRenderer = null;
        
        private IStatGeneric _iStatGeneric = null;
        private EState _eState = EState.None;
        
        public int Id
        {
            get { return id; }
        }

        // public Animator Animator { get; private set; } = null;
        public SpriteRenderer SpriteRenderer { get; private set; } = null;
        public SkeletonAnimation SkeletonAnimation { get; protected set; } = null;
        public Collider2D Collider { get; protected set; } = null;

        public Transform Transform
        {
            get { return NavMeshAgent?.transform; }
        }
        
        public NavMeshAgent NavMeshAgent { get; private set; } = null;

        public IStat IStat
        {
            get { return _iStatGeneric?.Stat; }
        }

        public Action.IActController IActCtr { get; protected set; } = null;
        
        public System.Action<IActor> EventHandler { get; private set; } = null;
        public float Height { get; private set; } = 0;

        #region Temp Stat
        [SerializeField] [Range(1f, 100f)] [Tooltip("이동 속도.")]
        private float moveSpeed = 1f;

        [SerializeField] 
        [Range(0f, 100f)]
        private float maxHp = 1f;
        #endregion

        public bool IsActivate
        {
            get
            {
                if (!rootTm)
                    return false;

                return rootTm.gameObject.activeSelf;
            }
        }

        public abstract string AnimationKey<T>(Act<T> act) where T : Act<T>.Param;
        
        #region ICharacterGeneric

        public virtual void Initialize()
        {
            NavMeshAgent = gameObject.GetComponentInChildren<NavMeshAgent>();
            if (NavMeshAgent != null)
            {
                NavMeshAgent.updateRotation = false;
                NavMeshAgent.updateUpAxis = false;
                NavMeshAgent.transform.localRotation = Quaternion.identity;
            }
            
            InitializeSkeletonAnimation();
            _meshRenderer = SkeletonAnimation?.GetComponent<MeshRenderer>();
            
            SpriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            
            Collider = GetComponentInChildren<Collider2D>(true);

            _iStatGeneric = new Stat();
            _iStatGeneric?.Initialize(this);

            SetOriginStat();

            IActCtr = transform.AddOrGetComponent<ActController>();
            IActCtr?.Initialize(this);

            Height = 150f;
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                Height += renderer.bounds.size.y;
                Height += heightOffest;
                Debug.Log(Height);
                //HeadPos = renderer.bounds.max;
            }
        }

        protected void InitializeSkeletonAnimation()
        {
            if (SkeletonAnimation != null)
                return;
            
            SkeletonAnimation = rootTm.GetComponentInChildren<SkeletonAnimation>(true);
            SkeletonAnimation?.Initialize(true);
        }

        public virtual void ChainUpdate()
        {
            if (!IsActivate)
                return;
            
            IActCtr?.ChainUpdate();
        }

        public virtual void ChainLateUpdate()
        {
            if (!IsActivate)
                return;

            IActCtr?.ChainLateUpdate();
        }
        
        public virtual void ChainFixedUpdate()
        {
            if (!IsActivate)
                return;

            IActCtr?.ChainFixedUpdate();
        }

        public virtual void Activate()
        {
            _iStatGeneric?.Activate();
            IActCtr?.Activate();

            Extensions.SetActive(rootTm, true);
        }

        public virtual void Deactivate(bool deactivateTm = false)
        {
            _iStatGeneric?.Deactivate();
            IActCtr?.Deactivate();

            EventHandler = null;

            if(deactivateTm)
                Extensions.SetActive(rootTm, false);
        }
        #endregion

        // public void EnableNavmeshAgent()
        // {
        //     NavMeshAgent = SkeletonAnimation?.AddOrGetComponent<NavMeshAgent>();
        //     if (NavMeshAgent != null)
        //     {
        //         NavMeshAgent.enabled = true;
        //
        //         NavMeshAgent.baseOffset = 0.5f;
        //         NavMeshAgent.speed = 3.5f;
        //         NavMeshAgent.angularSpeed = 200f;
        //         NavMeshAgent.acceleration = 100f;
        //         NavMeshAgent.radius = 0.5f;
        //         NavMeshAgent.height = 2f;
        //
        //         NavMeshAgent.updateRotation = false;
        //         NavMeshAgent.updateUpAxis = false;
        //
        //         NavMeshAgent.isStopped = false;
        //         NavMeshAgent.ResetPath();
        //     }
        // }

        // public void DisableNavmeshAgent()
        // {
        //     if (NavMeshAgent == null)
        //         return;
        //
        //     NavMeshAgent.isStopped = true;
        //     NavMeshAgent.enabled = false;
        // }
        
        #region ISubject

        public void SortingOrder(float order)
        {
            var sortingOrder = Mathf.CeilToInt(-order * 100f);
            
            if (SpriteRenderer != null)
                SpriteRenderer.sortingOrder = sortingOrder;

            if (_meshRenderer == null)
            {
                InitializeSkeletonAnimation();
                _meshRenderer = SkeletonAnimation?.GetComponent<MeshRenderer>();
            }
                
            if(_meshRenderer != null)
                _meshRenderer.sortingOrder = sortingOrder;
        }
        
        public int Order
        {
            get { return _meshRenderer.sortingOrder; }
        }
        #endregion
        
        #region IActor
        // public float Height
        // {
        //     get
        //     {
        //         float height = 100f;
        //         if (NavMeshAgent != null)
        //             height *= NavMeshAgent.height;
        //
        //         return height + heightOffest;
        //     }
        // }
        
        public void Flip(float x)
        {
            // if (SpriteRenderer != null)
            //     SpriteRenderer.flipX = x >= 0;

            if (SkeletonAnimation != null)
            {
                // 방향 전환 시, 튀는 현상때문에 추가.
                if (Mathf.Abs(x) > 0.01f)
                    SkeletonAnimation.Skeleton.ScaleX  = x > 0 ? -1f : 1f;
            }
        }
        
        void IActor.SetSkin(string skinName)
        {
            var skeleton = SkeletonAnimation?.Skeleton;
            if (skeleton?.Skin?.Name == skinName)
                return;
            
            skeleton?.SetSkin(skinName);
            skeleton?.SetSlotsToSetupPose();
            SkeletonAnimation?.AnimationState?.Apply(skeleton);
        }
        
        void IActor.SetEventHandler(System.Action<IActor> eventHandler)
        {
            EventHandler += eventHandler;
        }
        #endregion

        #region Temp Stat

        private void SetOriginStat()
        {
            IStat?.SetOrigin(Stat.EType.MoveSpeed, moveSpeed);
            IStat?.SetOrigin(Stat.EType.Hp, maxHp);
            IStat?.SetOrigin(Stat.EType.MaxHp, maxHp);
        }

        #endregion
        
        #region IState
        public void SetEState(EState eState)
        {
            if (eState == EState.None)
            {
                _eState = eState;
                return;
            }
            
            _eState |= eState;
        }
        
        public bool HasEState(EState eState)
        {
            return (_eState & eState) == eState;
        }

        public bool IsNone
        {
            get { return _eState == EState.None; }
        }
        #endregion
    }
}
