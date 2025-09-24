using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Creator;
using GameSystem;
using UI.Part;
using Creature.Characters;
using GameSystem.Event;

namespace Creature.Action
{
    public class Conversation : Act<Conversation.Param>, SpeechBubble.IListener
    {
        public new class Param : Act<Conversation.Param>.Param
        {
            public int TargetId { get; private set; } = 0;
            public Queue<(IActor, int)> Queue { get; private set; } = null;
            //public bool ExceptionNotify { get; private set; } = false;

            public GameSystem.Event.Quest QuestEventData { get; private set; } = null; 

            public Param WithTargetId(int targetId)
            {
                TargetId = targetId;
                return this;
            }
            
            public void Add(IActor targetIActor, int localId)
            {
                if (Queue == null)
                {
                    Queue = new();
                    Queue.Clear();
                }
                
                Queue?.Enqueue((targetIActor, localId));
            }
            
            public Param WithQuestEventData(GameSystem.Event.Quest questEventData)
            {
                QuestEventData = questEventData;
                return this;
            }
        }

        private SpeechBubble _speechBubble = null;
        
        public override void Execute()
        {
            base.Execute();
            
            if (_param == null)
                return;
  
            ActivateSpeechBubbleAsync(true).Forget();
        }
        
        public override void ChainLateUpdate()
        {
            base.ChainLateUpdate();
            
             _speechBubble?.ChainLateUpdate();
        }

        public override void End()
        {
            int targetId = _param.TargetId;
            var questEventData = _param.QuestEventData;
            
            base.End();
            
            _speechBubble?.Deactivate();

            if(questEventData != null)
                GameSystem.Event.EventDispatcher.Dispatch(questEventData);
        }

        private async UniTask ActivateSpeechBubbleAsync(bool isAppearEffect)
        {
            var queue = _param?.Queue;
            if (queue == null ||
                queue.Count <= 0)
            {
                End();
                return;
            }

            (IActor iActor, int talkLocalId) conversationData = (null, 0);
            if (_param.Queue.TryDequeue(out conversationData))
            {
                if (conversationData.Item1 == null)
                {
                    End();
                    return;
                }
            }

            var targetIActor = conversationData.iActor;
            int localId = conversationData.talkLocalId;

            var speechBubbleParam = new SpeechBubble.Param
            {
                TargetTm = targetIActor.Transform,
                Offset = new Vector2(10f, targetIActor.Height + 100f),
            };

            speechBubbleParam.SetIListener(this)?
                .SetText(localId.GetLocalization())?
                .SetIsAppearEffect(isAppearEffect);

            if (_speechBubble == null)
            {
                _speechBubble = await UICreator<SpeechBubble, SpeechBubble.Param>.Get
                    .SetParam(speechBubbleParam)
                    .SetWorldUI(true)
                    .CreateAsync();
            }
            else
                _speechBubble.ActivateWithParamAsync(speechBubbleParam).Forget();
        }
        
        #region
        void SpeechBubble.IListener.End()
        {
            ActivateSpeechBubbleAsync(false).Forget();
        }
        #endregion
    }
}
