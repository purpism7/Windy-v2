using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Common;
using Creator;
using Creature;
using Creature.Characters;
using GameSystem.Event;
using Info;
using Network;
using Network.Api.Quest;
using Table;
using Item = GameSystem.Event.Item;

namespace GameSystem.Mission
{
    public class Quest : MonoBehaviour, 
        IApiResponse<Network.Api.AddItem.Response>, 
        IApiResponse<Network.Api.RemoveItem.Response>,
        IApiResponse<Network.Api.Recipe.AddRecipe.Response>
    {
        public Table.QuestData CurrentQuestData { get; private set; } = null;

        public Quest Initialize()
        {
            SetCurrentQuest();
            
            Event.EventDispatcher.Register<Event.Quest>(OnCompleted);
            
            return this;
        }

        private void SetCurrentQuest()
        {
            var questInfo = InfoManager.Get<Info.Quest>();
            if (questInfo == null)
                return;
  
            CurrentQuestData = QuestDataContainer.Instance?.GetData(questInfo.Group, questInfo.Step);
        }

        public void SetNextQuest(int questGroup, int nextQuestStep)
        {
            if (CurrentQuestData == null)
                return;

            // var questGroup = CurrentQuestData.Group;
            // var nextQuestStep = CurrentQuestData.Step + 1;
            var questData = QuestDataContainer.Instance?.GetData(questGroup, nextQuestStep);
            if (questData == null)
            {
                // 다음 step 이 없기 때문에, 다음 quest 로 넘어가기.
                var nextQuestGroup = questGroup + 1;
                nextQuestStep = 1;
                
                CurrentQuestData = QuestDataContainer.Instance?.GetData(nextQuestGroup, nextQuestStep);
            }
            else
                CurrentQuestData = questData;
            
            Notify();
        }

        private void Notify()
        {
            if (CurrentQuestData == null)
                return;
            
            Event.EventDispatcher.Dispatch(new Event.ChangeQuest());
        }
        
        private void RequestSaveQuest(int questGroup, int questStep)
        {
            var request = SaveQuest.CreateRequest<SaveQuest>(
                new SaveQuest.Request
                {
                    QuestGroup = questGroup,
                    QuestStep = questStep,
                });
                    
            ApiClient.Instance?.RequestPost(request);
        }

        private bool IsCompletedTalkNpc(int npcId)
        {
            var condition = CurrentQuestData?.ConditionList?.FirstOrDefault();
            if (condition == null)
                return false;
                    
            if (condition.Value.Item1 != EMissionCondition.TalkNpc)
                return false;
            
            if (CurrentQuestData.NpcId != npcId)
                return false;
            
            return true;
        }

        public bool IsCompletedQuestClear(int npcId)
        {
            var conditionList = CurrentQuestData?.ConditionList;
            if (conditionList == null)
                return false;
            
            var questNpcId = CurrentQuestData.NpcId;
            if (questNpcId <= 0 ||
                questNpcId != npcId)
                return false;

            bool isCompleted = true;
            
            for (int i = 0; i < conditionList.Count; ++i)
            {
                var condition = conditionList[i];
                if (condition.Item1 == EMissionCondition.BringItem)
                {
                    int itemId = condition.Item2.FirstOrDefault();
                    int reqItemCount = condition.Item2.LastOrDefault();
                    var itemCount = InfoManager.Instance.GetItemCount(itemId);

                    if (reqItemCount > itemCount)
                    {
                        isCompleted = false;
                        break;
                    }
                }
            }

            return isCompleted;
        }

        private Dictionary<int, int> BringItemDic
        {
            get
            {
                var conditionList = CurrentQuestData?.ConditionList;
                if (conditionList.IsNullOrEmpty())
                    return null;

                Dictionary<int, int> dic = new();
                dic.Clear();
                
                for (int i = 0; i < conditionList?.Count; ++i)
                {
                    var condition = conditionList[i];
                    if (condition.Item1 == EMissionCondition.BringItem)
                    {
                        int itemId = condition.Item2.FirstOrDefault();
                        int itemCount = condition.Item2.LastOrDefault();

                        if (dic.ContainsKey(itemId))
                            dic[itemId] = itemCount;
                        else 
                            dic.TryAdd(itemId, itemCount);
                    }
                }

                return dic;
            }
        }
        
        private void RequestAddItem(int itemId, int itemCount)
        {
            var request = Network.Api.AddItem.CreateRequest<Network.Api.AddItem>(
                new Network.Api.AddItem.Request
                {
                    ItemId = itemId,
                    ItemCount = itemCount,
                });
                    
            ApiClient.Instance?.RequestPost(request, this);
        }
        
        private void RequestRemoveItem(Dictionary<int, int> itemDic, System.Action<System.Action> completedAction)
        {
            var request = Network.Api.RemoveItem.CreateRequest<Network.Api.RemoveItem>(
                new Network.Api.RemoveItem.Request
                {
                    ItemDic = itemDic,
                    CompletedAction = completedAction,
                });
                    
            ApiClient.Instance?.RequestPost(request, this);
        }

        private void RequestAddRecipe(int[] recipeIds)
        {
            var request = Network.Api.Recipe.AddRecipe.CreateRequest<Network.Api.Recipe.AddRecipe>(
                new Network.Api.Recipe.AddRecipe.Request
                {
                    RecipeIds = recipeIds,
                });
                    
            ApiClient.Instance?.RequestPost(request, this);
        }

        private async UniTask GetRewardAsync()
        { 
            if (CurrentQuestData.RewardId <= 0)
                return;

            //var rewardIds = CurrentQuestData?.RewardIds;
            //if (rewardIds.IsNullOrEmpty())
            //    return;

            //for (int i = 0; i < rewardIds?.Length; ++i)
            //{
                //int rewardId = rewardIds[i];
                var rewardData = RewardDataContainer.Instance?.GetData(CurrentQuestData.RewardId);
                if(rewardData == null)
                    return;
                
                RequestAddItem(rewardData.ItemId, rewardData.ItemCount);

            //    if (rewardIds.Length > i)
            //        await UniTask.Yield();
            //}
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }

        private void GetRecipe()
        {
            if (CurrentQuestData == null)
                return;
            
            var recipeIdList = CurrentQuestData.RecipeIds?.ToList();
            if (recipeIdList.IsNullOrEmpty())
                return;
            
            for (int i = recipeIdList.Count - 1; i >= 0 ; --i)
            {
                int recipeId = recipeIdList[i];
                var recipeData = RecipeDataContainer.Instance.GetData(recipeId);
                if (recipeData == null)
                    recipeIdList.RemoveAt(i);
            }
            
            RequestAddRecipe(recipeIdList.ToArray());
        }
        
        private async UniTask GetItemAsync(int itemId)
        {
            var playable = CharacterManager.Playable;
            if (playable == null)
                return;
            
            playable.SetEState(EState.GetItem);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            
            var getItemParam = new UI.Part.GetItem.Param
            {
                TargetTm = playable.Transform,
                Offset = new Vector2(0, playable.Height - 10f),
            }.WithItemId(itemId)?
            .WithCallback(
                () =>
                {
                    playable.SetEState(EState.None);
                });
                
            var getItem = await UICreator<UI.Part.GetItem, UI.Part.GetItem.Param>.Get
                .SetParam(getItemParam)
                .SetWorldUI(true)
                .CreateAsync();
               
            // getItem?.Activate(getItemParam);
        }
        
        public bool CheckAndProceedQuest(int npcId, Action<Action> completeAction)
        {
            if (IsCompletedQuestClear(npcId))
            {
                var bringItem = new GameSystem.Event.BringItem();
                bringItem.WithCompletedAction(completeAction);

                GameSystem.Event.EventDispatcher.Dispatch<Event.Quest>(bringItem);
                return true;
            }
               
            return false;
        }

        private async UniTask ClearQuestAsync()
        {
            await GetRewardAsync();
            GetRecipe();
            
            var questGroup = CurrentQuestData.Group;
            var nextQuestStep = CurrentQuestData.Step + 1;
            SetNextQuest(questGroup, nextQuestStep);
            
            if (CurrentQuestData != null)
                RequestSaveQuest(CurrentQuestData.Group, CurrentQuestData.Step);
        }

        private void OnCompleted(Event.Quest quest)
        {
            if (CurrentQuestData == null)
                return;
            
            switch (quest)
            {
                case TalkNpc talkNpc:
                {
                    if (IsCompletedTalkNpc(talkNpc.NpcId))
                        ClearQuestAsync().Forget();
                    
                    break;
                }

                case BringItem bringItem:
                {
                    RequestRemoveItem(BringItemDic, bringItem.CompletedAction);
                    break;
                }

                case PathFindPuzzle pathFindPuzzle:
                {
                    Debug.Log("Here");
                    if (pathFindPuzzle.IsClear)
                        ClearQuestAsync().Forget();
                    
                    break;
                }
            }
        }

        #region IApiResponse
        void IApiResponse<Network.Api.AddItem.Response>.OnResponse(Network.Api.AddItem.Response data, bool isSuccess, string errorMessage)
        {
            if (isSuccess)
            {
                var addItemData = new GameSystem.Event.AddItem(data.ItemId, data.ItemCount);
                GameSystem.Event.EventDispatcher.Dispatch<Item>(addItemData);
                GetItemAsync(data.ItemId).Forget();
            }
        }
        
        void IApiResponse<Network.Api.RemoveItem.Response>.OnResponse(Network.Api.RemoveItem.Response data, bool isSuccess, string errorMessage)
        {
            if (isSuccess)
            {
                GameSystem.Event.EventDispatcher.Dispatch<Item>(new GameSystem.Event.RemoveItem(data.ItemDic));
                data?.CompletedAction?.Invoke(
                    () =>
                    {
                        ClearQuestAsync().Forget();
                    });
            }
        }

        void IApiResponse<Network.Api.Recipe.AddRecipe.Response>.OnResponse(
            Network.Api.Recipe.AddRecipe.Response response, bool isSuccess, string errorMessage)
        {
            if (isSuccess)
            {
                GameSystem.Event.EventDispatcher.Dispatch<Event.Recipe>(new GameSystem.Event.AddRecipe());
            }
        }
        #endregion
    }
}
