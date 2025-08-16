using UnityEngine;
using UnityEngine.Playables;

public interface ICutscene
{
    void Intialize(Cutscene.IListener iListener);
    void Play(PlayableDirector playableDirector);
}

public class Cutscene : MonoBehaviour, ICutscene
{
    public interface IListener
    {
        void Finish();
    }

    [SerializeField] private int id = 0;
    [SerializeField] private PlayableAsset asset = null;

    private IListener _iListener = null;

    public PlayableAsset PlayableAsset => asset;

    void ICutscene.Intialize(IListener iListener)
    {
        _iListener = iListener;

        Extensions.SetActive(transform, false);
    }

    void ICutscene.Play(PlayableDirector playableDirector)
    {
        if (playableDirector == null)
        {
            _iListener?.Finish();
            return;
        }

        Extensions.SetActive(transform, true);

        playableDirector.playableAsset = asset;
        playableDirector.Play();
    }
}
