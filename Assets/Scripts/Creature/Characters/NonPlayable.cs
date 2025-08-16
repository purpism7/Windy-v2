using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

using Cysharp.Threading.Tasks;

using Creature.Action;
using GameSystem;
using Table;
using GameSystem.Event;


namespace Creature.Characters
{
    public interface INonPlayable
    {
        int Id { get; }
        int[] TalkLocalIds { get; }
        int[] RefreshTalkLocalIds();
    }
    
    public class NonPlayable : Character, INonPlayable, Act<Move.Param>.IListener
    {
        [SerializeField] private bool autoMove = false;

        private const float MoveRange = 3f;
        
        private int[] _talkIds = null;
        private Vector3 _originPos = Vector3.zero;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!autoMove)
                return;                
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_originPos, MoveRange);
        }
#endif
        
        public override void Initialize()
        {
            base.Initialize();
            
            if(Transform)
                SortingOrder(Transform.position.y);

            if (autoMove)
                _originPos = Transform.position;
            
            GameSystem.Event.EventDispatcher.Register<GameSystem.Event.ChangeQuest>(OnChanged);
            //IEventHandler a = new GameSystem.Event.EventHandler<As>();
            ////a.Add(OnChanged);
            Activate();
        }

        public override void Activate()
        {
            base.Activate();

            if (autoMove)
                RandomMoveAsync().Forget();
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();
        }
        
        public override string AnimationKey<T>(Act<T> act)
        {
            switch (act)
            {
                case Move:
                    return "walk";
                
                case Idle:
                    return nameof(Idle).ToLower();
            }
            
            return act?.GetType().Name;
        }

        private async UniTask RandomMoveAsync()
        {
            var delaySec = UnityEngine.Random.Range(10f, 20f);
            await UniTask.Delay(TimeSpan.FromSeconds(delaySec));
            
            var currIAct = IActCtr?.CurrIAct;
            if (currIAct is Conversation)
                return;
            
            float value = MoveRange;
            float randomX = UnityEngine.Random.Range(-value, value);
            float randomY = UnityEngine.Random.Range(-value, value);
            var targetPos = new Vector3(_originPos.x + randomX, _originPos.y + randomY, 0);

            NavMeshHit hit;
            if (!NavMesh.SamplePosition(targetPos, out hit, 1f, NavMesh.AllAreas))
            {
                RandomMoveAsync().Forget();
                return;
            }
            
            targetPos = hit.position;
            targetPos.z = 0;

            IActCtr?.MoveToTarget(targetPos);
        }

        #region INonPlayable
        public int[] TalkLocalIds
        {
            get
            {
                var currentQuestData = Manager.Get<IMission>()?.Quest?.CurrentQuestData;
                if (currentQuestData == null)
                    return null;

                var talkData = TalkDataContainer.Instance?.GetData(Id, currentQuestData.Group, currentQuestData.Step);
                if (talkData == null)
                    return null;
                
                if (!_talkIds.IsNullOrEmpty() &&
                    _talkIds.SequenceEqual(talkData.TalkLocalIds))
                    return new[] { _talkIds.LastOrDefault() };
                
                return _talkIds;
            }
        }

        public int[] RefreshTalkLocalIds()
        {
            var currentQuestData = Manager.Get<IMission>()?.Quest?.CurrentQuestData;
            if (currentQuestData == null)
                return null;

            var talkData = TalkDataContainer.Instance?.GetData(Id, currentQuestData.Group, currentQuestData.Step);
            if (talkData == null)
                return null;

            if (!_talkIds.IsNullOrEmpty() &&
                _talkIds.SequenceEqual(talkData.TalkLocalIds))
                return new[] { _talkIds.LastOrDefault() };

            _talkIds = talkData.TalkLocalIds;

            return _talkIds;
        }
        #endregion
        
        void Act<Move.Param>.IListener.End()
        {
            IActCtr?.ExecuteAsync<Idle, Idle.Param>().Forget();
            RandomMoveAsync().Forget();
        }
        #region

        private void OnChanged(GameSystem.Event.ChangeQuest questClear)
        {
            _talkIds = null;
        }
        #endregion
    }
}

