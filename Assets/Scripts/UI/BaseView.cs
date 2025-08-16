using GameSystem;
using UnityEngine;

namespace UI
{
    public abstract class BaseView<T> : Common.Component<T> where T : Common.Component<T>.Param
    {
        public new class Param : Common.Component<T>.Param
        {

        }

        public abstract void CreatePresenter();

        public override void Deactivate()
        {
            base.Deactivate();

            UIManager.Instance?.ClearCurrIView();
        }
    }
}

