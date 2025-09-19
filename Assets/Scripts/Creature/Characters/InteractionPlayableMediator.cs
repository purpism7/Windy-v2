using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Creator;
using Creature.Action;
using GameSystem;
using GameSystem.Event;
using Network;
using Table;
using UI.Part;
using Common;
using AddItem = Network.Api.AddItem;

namespace Creature.Characters
{
    public class InteractionPlayableMediator : InteractionMediator<Playable>, Act<Conversation.Param>.IListener, IApiResponse<AddItem.Response>
    {
        private NonPlayable _interactionNpc = null;
        private UI.Part.Interaction _interactionPart = null;
        private IObject _interactionIObj = null;
        private System.Action completedActionCallback = null;

        private EItemInteraction _eItemInteraction = EItemInteraction.None;
        
        public bool IsInteracting { get; private set; } = false;
        
        public override void Initialize(Playable playable)
        {
            base.Initialize(playable);

            foreach (EItemInteraction eItemInteraction in Enum.GetValues(typeof(EItemInteraction)))
            {
                AddEInteractableAction(eItemInteraction);
            }
            
            GameSystem.Event.EventDispatcher.Register<GameSystem.Event.Item>(OnChanged);
        }
        
        private void AddEInteractableAction(EItemInteraction eItemInteraction)
        {
            if ((_eItemInteraction & eItemInteraction) == eItemInteraction)
                return;
            
            if(InfoManager.Instance.HasItem(eItemInteraction))
                _eItemInteraction |= eItemInteraction;
        }

        public override void ChainLateUpdate()
        {
            base.ChainLateUpdate();
            
            UpdateIsNearNpc();
        }

        private void UpdateIsNearNpc()
        {
            if (IsInteracting)
                return;
            
            var iActCtr = _t?.IActCtr;
            if (iActCtr == null)
                return;

            if (!_t.IsNone)
                return;
            
            if (iActCtr.IsInteraction)
                return;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(_t.Transform.position, 1f, LayerMask.GetMask("Character"));
            if (colliders == null)
                return;

            NonPlayable nearestNpc = null;
            for (int i = 0; i < colliders.Length; ++i)
            {
                var collider = colliders[i];
                if (collider == null)
                    continue;

                var nonPlayable = collider.gameObject.GetComponentInParent<NonPlayable>();
                if(nonPlayable == null)
                    continue;

                nearestNpc = nonPlayable;
                break;
            }
            
            if (nearestNpc != null  &&
                _interactionNpc == null)
            {
                _interactionNpc = nearestNpc;
                ActivateInteractionPartAsync().Forget();
            }
            else if (nearestNpc == null  &&
                     _interactionNpc != null)
            {
                _interactionNpc = null;
                _interactionPart?.Deactivate();
            }
        }
        
        private async UniTask ActivateInteractionPartAsync()
        {
            await CreateInteractionPartAsync();

            var eInteraction = EInteraction.Talk;
            var quest = Manager.Get<IMission>()?.Quest;
            if (quest != null &&
                quest.CurrentQuestData != null)
            {
                if (quest.IsCompletedQuestClear(_interactionNpc.Id))
                    eInteraction = EInteraction.QuestClear;

                if (eInteraction == EInteraction.Talk)
                {
                    var talkData = TalkDataContainer.Instance?.GetData(_interactionNpc.Id, quest.CurrentQuestData.Group, quest.CurrentQuestData.Step);
                    if (talkData == null)
                        return;

                    if (talkData.TalkLocalIds.IsNullOrEmpty())
                        return;
                }
            }

            var interactionParam = new Interaction.Param
            {
                TargetTm = _t.Transform,
                Offset = new Vector2(0, _t.Height),
            }.WithEInteraction(eInteraction);
     
            await _interactionPart.ActivateWithParamAsync(interactionParam);
        }
        
        private async UniTask CreateInteractionPartAsync()
        {
            if (_t == null)
                return;
            
            if (_interactionPart != null)
                return;
            
            var interactionParam = new Interaction.Param
            {
                TargetTm = _t.Transform,
                Offset = new Vector2(0, _t.Height),
            };

            _interactionPart = await UICreator<Interaction, Interaction.Param>.Get
                .SetParam(interactionParam)
                .SetWorldUI(true)
                .CreateAsync();
        }
        
        private bool InteractionWithNpc(NonPlayable nonPlayable)
        {
            var iActCtr = _t?.IActCtr;
            if (iActCtr == null)
                return false;
            
            if (iActCtr.IsInteraction)
                return false;

            var quest = Manager.Get<IMission>().Quest;
            var currentQuestData = quest?.CurrentQuestData;
            if (quest == null ||
                currentQuestData == null)
                return false;
            
            var talkData = TalkDataContainer.Instance?.GetData(nonPlayable.Id, currentQuestData.Group, currentQuestData.Step);
            if (talkData == null)
                return false;
            
            var talkLocalIds = nonPlayable.TalkLocalIds;
            if (!talkLocalIds.IsNullOrEmpty() &&
                !talkLocalIds.SequenceEqual(talkData.TalkLocalIds))
            {
                if (CanOpenPathFindPuzzlePanel(currentQuestData))
                {
                    OpenPathFindPuzzlePanel(currentQuestData).Forget();
                    return false;
                }

                if (quest.CheckAndProceedQuest(nonPlayable.Id,
                        (callback) =>
                        {
                            completedActionCallback = callback;

                            var talkData = TalkDataContainer.Instance.GetData(currentQuestData.CompleteTalkId);
                            TalkWithNpc(nonPlayable, talkData.TalkLocalIds, true);
                        }))
                    return true;
            }
            else
                talkLocalIds = nonPlayable.RefreshTalkLocalIds();
            
            return TalkWithNpc(nonPlayable, talkLocalIds, false);
        }

        private bool TalkWithNpc(NonPlayable nonPlayable, int[] talkLocalIds, bool exceptionNotify)
        {
            if (talkLocalIds.IsNullOrEmpty())
                return false;

            var conversationParam = new Conversation.Param
            {
                IListener = this,
            }.WithTargetId(nonPlayable.Id)
            .WithExceptionNotify(exceptionNotify);

            for (int i = 0; i < talkLocalIds?.Length; ++i)
            {
                if(talkLocalIds[i] <= 0)
                    continue;

                conversationParam.Add(nonPlayable, talkLocalIds[i]);
            }
 
            _t?.IActCtr?.ExecuteAsync<Conversation, Conversation.Param>(conversationParam).Forget();

            return true;
        }
        
        private void UpdateInteractionObject()
        {
            var iActCtr = _t?.IActCtr;
            if (iActCtr == null)
                return;
            
            if (iActCtr.IsInteraction)
                return;
            
            if (_interactionIObj == null)
                return;
            
            if (!_interactionIObj.IsActivate)
                return;
            
            if (_interactionIObj.EItemInteraction == EItemInteraction.None)
                return;
            
            if ((_eItemInteraction & _interactionIObj.EItemInteraction) != _interactionIObj.EItemInteraction)
                return;

            var cutDownParam = new CutDown.Param()
                .SetIObject(_interactionIObj);

            iActCtr.ExecuteAsync<Creature.Action.CutDown, Creature.Action.CutDown.Param>(cutDownParam).Forget();
            
            // 타겟을 바라보고 Act 진행.
            var objTm = _interactionIObj?.Transform;
            if(objTm)
                _t?.Flip(objTm.position.x < _t.Transform.position.x ? -Mathf.Abs(_t.Transform.localScale.x) : Mathf.Abs(_t.Transform.localScale.x));
        }

        private void UpdatePickUp()
        {
             var iActCtr = _t?.IActCtr;
            if (iActCtr == null)
                return;
            
            if (iActCtr.IsInteraction)
                return;
            
            if (_interactionIObj == null)
                return;
            
            if (!_interactionIObj.IsActivate)
                return;

            if (_interactionIObj.EItem != EItem.Material)
                return;
            
            iActCtr.ExecuteAsync<PickUp, PickUp.Param>(
                new PickUp.Param
                {
                    IObject = _interactionIObj, 
                }).Forget();

            RequestAddItem(_interactionIObj.Id, 1);
            
            // 타겟을 바라보고 Act 진행.
            var objTm = _interactionIObj?.Transform;
            if(objTm)
                _t?.Flip(objTm.position.x < _t.Transform.position.x ? -Mathf.Abs(_t.Transform.localScale.x) : Mathf.Abs(_t.Transform.localScale.x));
        }
        
        private void RequestAddItem(int itemId, int itemCount)
        {
            var request = AddItem.CreateRequest<AddItem>(
                new AddItem.Request
                {
                    ItemId = itemId,
                    ItemCount = itemCount,
                });
                    
            ApiClient.Instance?.RequestPost(request, this);
        }

        private void StartInteraction(GameObject gameObj)
        {
            if (!gameObj)
                return;
            
            if (_interactionIObj != null)
                return;
            
            var iObj = gameObj.GetComponentInParent<IObject>();
            if (iObj == null)
                return;

            if (!iObj.IsActivate)
                return;
            
            _interactionIObj = iObj;
        }

        private bool CanOpenPathFindPuzzlePanel(QuestData questData)
        {
            if (questData == null)
                return false;
            
            if (questData.EMissionCondition1 != EMissionCondition.PathFindPuzzle)
                return false;

            return true;
        }
        private async UniTask<bool> OpenPathFindPuzzlePanel(QuestData questData)
        {
            int puzzleIndex = questData.Value1.FirstOrDefault();
            
            var pathFindPuzzleView = await UICreator < UI.Puzzle.PathFindPuzzleView, UI.Puzzle.PathFindPuzzleView.Param>.Get
                .SetRootTm(UIManager.Instance?.ViewUIRootRectTm)
                .SetParam(new UI.Puzzle.PathFindPuzzleView.Param(puzzleIndex))
                .SetInitializeSize(true)
                .CreateAsync();

            return pathFindPuzzleView != null;
        }
        
        void Act<Conversation.Param>.IListener.End()
        {
            IsInteracting = false;
            completedActionCallback?.Invoke();
            completedActionCallback = null;
        }

        private async UniTask GetItemAsync(int itemId)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            
            var getItemParam = new GetItem.Param
            {
                TargetTm = _t.Transform,
                Offset = new Vector2(0, _t.Height - 10f),
            }.WithItemId(itemId);
                
            var getItem = await UICreator<GetItem, GetItem.Param>.Get
                .SetParam(getItemParam)
                .SetWorldUI(true)
                .CreateAsync();
               
            // getItem?.Activate(getItemParam);
        }
        
        #region TriggerEvent
        public override void OnEnter(GameObject gameObj)
        {
            StartInteraction(gameObj);
        }

        public override void OnExit(GameObject gameObj)
        {
            _interactionIObj = null;
        }

        public override void OnStay(GameObject gameObj)
        {
            base.OnStay(gameObj);

            if (_interactionIObj == null)
            {
                StartInteraction(gameObj);
                return;
            }
            else
            {
                if (!_interactionIObj.IsActivate)
                {
                    _interactionIObj = null;
                    return;
                }
            }
        }

        #endregion 
        
        #region InputManager.IListener
        public override void OnKey(KeyCode keyCode)
        {
            if (IsInteracting)
                return;
            
            switch (keyCode) 
            {
                case KeyCode.Space:
                {
                    if (_interactionIObj == null)
                        return;
                    
                    UpdateInteractionObject();
                    break;
                }
            }
        }
        
        public override void OnKeyDown(KeyCode keyCode)
        {
            base.OnKeyDown(keyCode);
            
            if (IsInteracting)
                return;
            
            switch (keyCode) 
            {
                case KeyCode.Space:
                {
                    if (_interactionNpc != null)
                    {
                        if (InteractionWithNpc(_interactionNpc))
                        {
                            IsInteracting = true;
                            _interactionPart?.Deactivate();
                            _interactionNpc = null;
                        }
                        return;
                    }

                    if (_interactionIObj != null)
                    {
                        UpdatePickUp();
                        return;
                    }
                    
                    break;
                }
            }
            
            if (_interactionNpc != null)
            {
                if (keyCode == KeyCode.Space)
                {
                    IsInteracting = true;
                    _interactionPart?.Deactivate();
                    InteractionWithNpc(_interactionNpc);

                    _interactionNpc = null;
                    return;
                }
            }
        }
        #endregion
        
        #region GameSystem.Event
        private void OnChanged(GameSystem.Event.Item item)
        {
            switch (item)
            {
                case GameSystem.Event.AddItem addItem:
                {
                    var itemData = ItemDataContainer.Instance.GetData(addItem.Id);
                    if (itemData != null &&
                        itemData.EItemInteraction != EItemInteraction.None)
                        AddEInteractableAction(itemData.EItemInteraction);

                    break;
                }
            }
        }
        #endregion
        
        #region IApiResponse
        void IApiResponse<AddItem.Response>.OnResponse(AddItem.Response data, bool isSuccess, string errorMessage)
        {
            if (isSuccess)
            {
                GameSystem.Event.EventDispatcher.Dispatch<Item>(new GameSystem.Event.AddItem(data.ItemId, data.ItemCount));

                GetItemAsync(data.ItemId).Forget();
            }
        }
        #endregion 
    }
}

