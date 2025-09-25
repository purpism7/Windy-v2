using UnityEngine;
using System;

using Common;
using GameSystem;


public class Visibility : Common.Component
{
    [Serializable]
    public class Condition
    {
        public VisibilityCondition VisibilityCondition = VisibilityCondition.None;
        public VisibilityPhase VisibilityPhase = VisibilityPhase.None;
        public Common.Visibility Visibility = Common.Visibility.None;

        public int QuestGroup = 0;
        public int QuestStep = 0;

        public EWeather Weather = EWeather.None;

        public ETimeOfDay TimeOfDay = ETimeOfDay.None;
    }

    [SerializeField]
    private Condition[] conditions = null;
   

    public override void Initialize()
    {
        base.Initialize();


    }

    private bool CheckCondition()
    {
        if (conditions.IsNullOrEmpty())
            return false;

        for(int i = 0; i < conditions.Length; ++i)
        {
            var condition = conditions[i];
            if (condition == null)
                continue;

            switch (condition.VisibilityCondition)
            {
                case VisibilityCondition.None:
                    continue;

                case VisibilityCondition.Quest:
                    if (CheckQuest(condition))
                        return true;
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

        return false;
    }

    private bool CheckQuest(Condition condition)
    {
        var questData = Manager.Get<IMission>()?.Quest?.CurrentQuestData;
        if (questData == null)
            return false;

        switch(condition.VisibilityPhase)
        {
            case VisibilityPhase.Before:
                {
                    if(questData.Group <= condition.QuestGroup &&
                       questData.Step < condition.QuestStep)
                    {
                        if(condition.Visibility == Common.Visibility.Visible)

                    }

                    break;
                }
        }

        return false;
    }

    private void Set()
    {

    }
}
