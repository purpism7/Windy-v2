using GameSystem;
using GameSystem.Event;
using UnityEngine;

namespace Creature
{
    public class Bridge : Object
    {
        [SerializeField] private Transform bridgeTm = null;
        [SerializeField] private Transform brokenBridgeTm = null;

        protected override void Initialize()
        {
            base.Initialize();

            var currQuestData = Manager.Get<IMission>()?.Quest.CurrentQuestData;
            if (currQuestData != null)
            {
                // 수정해야됨.
                if (currQuestData.Group >= 2 ||
                    currQuestData.Group >= 1 && currQuestData.Step >= 7)
                    ActivateFixedBridge();
                else
                    GameSystem.Event.EventDispatcher.Register<GameSystem.Event.Quest>(OnChangedEvent);
            }
        }

        private void ActivateFixedBridge()
        {
            Extensions.SetActive(bridgeTm, true);
            Extensions.SetActive(brokenBridgeTm, true);
            
            MainManager.Instance?.NavMeshSurface?.BuildNavMesh();
        }

        private void OnChangedEvent(GameSystem.Event.Quest data)
        {
            switch (data)
            {
                case PathFindPuzzle pathFindPuzzle:
                {
                    //if (pathFindPuzzle.IsClear)
                    //{
                    //    Extensions.SetActive(bridgeTm, true);
                    //    Extensions.SetActive(brokenBridgeTm, true);
                    //}
                    break;
                }
            }
        }
    }
}

