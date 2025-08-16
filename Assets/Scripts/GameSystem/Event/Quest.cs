using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Event
{
    public class Quest : EventParam
    {
        
    }

    public sealed class TalkNpc : Quest
    {
        public int NpcId { get; private set; } = 0;
        
        public TalkNpc(int npcId)
        {
            NpcId = npcId;
        }
    }

    public sealed class BringItem : Quest
    {
        public System.Action<System.Action> CompletedAction { get; private set; } = null;
        
        public BringItem WithCompletedAction(System.Action<System.Action> completedAction)
        {
            CompletedAction = completedAction;
            return this;
        }
    }
    
    public sealed class PathFindPuzzle : Quest
    {
        public bool IsClear { get; private set; } = false;

        public PathFindPuzzle WithIsClear(bool isClear)
        {
            IsClear = isClear;
            return this;
        }
    }

    public class ChangeQuest : EventParam
    {
        public int QuestId
        {
            get { return Manager.Get<IMission>()?.Quest?.CurrentQuestData?.Group ?? 0; }
        }

        public int QuestStep
        {
            get { return Manager.Get<IMission>()?.Quest?.CurrentQuestData?.Step ?? 0; }
        }
    }
}

