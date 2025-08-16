using UnityEngine;

using Cysharp.Threading.Tasks;

using Common;

namespace UI.Part
{
    public class Interaction : PartWorld<Interaction.Param>
    {
        public new class Param : PartWorld<Interaction.Param>.Param
        {
            public EInteraction EInteraction { get; private set; } = EInteraction.None;

            public Param WithEInteraction(EInteraction eInteraction)
            {
                EInteraction = eInteraction;
                return this;
            }
        }

        [SerializeField] private Transform talkTm = null;
        [SerializeField] private Transform questClearTm = null;

        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
        }

        public override async UniTask ActivateAsync()
        {
            await base.ActivateAsync();
            
            Extensions.SetActive(talkTm, _param.EInteraction == EInteraction.Talk);
            Extensions.SetActive(questClearTm, _param.EInteraction == EInteraction.QuestClear);

            AppearEffectAsync().Forget();
            await UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            
            
        }
    }
}

