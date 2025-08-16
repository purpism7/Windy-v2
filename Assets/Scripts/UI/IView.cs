using UnityEngine;

namespace UI
{
    public interface IView
    {
        void Activate();
        void Deactivate();
        void ChainUpdate();
    }
}
