using UnityEngine;
using System;

using Common;
using GameSystem;
using GameSystem.Event;
using Cysharp.Threading.Tasks;

namespace Common
{
    public class Visibility : Common.Component
    {
        [Serializable]
        public class Condition
        {
            public VisibilityCondition VisibilityCondition = VisibilityCondition.None;
            public VisibilityPhase VisibilityPhase = VisibilityPhase.None;

            public int QuestGroup = 0;
            public int QuestStep = 0;

            public int ToQuestGroup = 0;
            public int ToQuestStep = 0;

            public EWeather Weather = EWeather.None;

            public ETimeOfDay TimeOfDay = ETimeOfDay.None;
        }

        [SerializeField] private bool isAutoStart = false;
        [SerializeField] private Transform targetTm = null;
        [SerializeField] private VisibilityType visibilityType = Common.VisibilityType.None;
        [SerializeField] private Condition[] conditions = null;

        private void Awake()
        {
            if (isAutoStart)
                Initialize();
        }

        private void OnEnable()
        {
            if (isAutoStart)
                Activate();
        }

        public override void Initialize()
        {
            base.Initialize();

            GameSystem.Event.EventDispatcher.Register<GameSystem.Event.ChangeQuest>(OnChangedEvent);
        }

        public override void Activate()
        {
            base.Activate();

            VisibilityAsync().Forget();
        }

        private async UniTask VisibilityAsync()
        {
            bool condition = await CheckConditionAsync();
            Extensions.SetActive(targetTm, visibilityType == VisibilityType.Visible ? condition : !condition);
        }

        private async UniTask<bool> CheckConditionAsync()
        {
            if (conditions.IsNullOrEmpty())
                return false;

            for (int i = 0; i < conditions.Length; ++i)
            {
                var condition = conditions[i];
                if (condition == null)
                    continue;

                switch (condition.VisibilityCondition)
                {
                    case VisibilityCondition.None:
                        continue;

                    case VisibilityCondition.Quest:
                        if (!await CheckQuestAsync(condition))
                            return false;

                        break;

                        //case VisibilityCondition.Weather:
                        //    if (CheckWeather(condition.Weather))
                        //        return true;
                        //    break;

                        //case VisibilityCondition.TimeOfDay:
                        //    if (CheckTimeOfDay(condition.TimeOfDay))
                        //        return true;
                        //    break;
                }
            }

            return true;
        }

        private async UniTask<bool> CheckQuestAsync(Condition condition)
        {
            await UniTask.WaitUntil(() => Manager.Get<IMission>()?.Quest?.CurrentQuestData != null);

            var questData = Manager.Get<IMission>()?.Quest?.CurrentQuestData;
            if (questData == null)
                return false;

            switch (condition.VisibilityPhase)
            {
                case VisibilityPhase.Before:
                    {
                        if (questData.Group <= condition.QuestGroup &&
                            questData.Step <= condition.QuestStep)
                            return true;

                        break;
                    }

                case VisibilityPhase.During:
                    {
                        if (condition.QuestGroup <= questData.Group &&
                            condition.ToQuestGroup >= questData.Group)
                        {
                            if (condition.QuestStep <= questData.Step &&
                                condition.ToQuestStep >= questData.Step)
                                return true;
                        }

                        break;
                    }

                case VisibilityPhase.After:
                    {
                        if (questData.Group > condition.QuestGroup)
                            return true;

                        if (questData.Group == condition.QuestGroup)
                        {
                            if (questData.Step > condition.QuestStep)
                                return true;
                        }

                        break;
                    }
            }

            return false;
        }

        private void OnChangedEvent(ChangeQuest eventParam)
        {
            VisibilityAsync().Forget();
        }
    }
}

