
using Cysharp.Threading.Tasks;

namespace UI
{
    public interface IPresenter<T, V> where V : IView
    {
        T Initialize(V iView);
        UniTask ActivateAsync();
        UniTask DeactivateAsync();

        void ChainUpdate();
    }
}
