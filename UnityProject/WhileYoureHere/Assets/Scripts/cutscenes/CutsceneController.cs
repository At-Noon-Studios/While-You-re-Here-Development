using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    
    void Awake()
    {
        // Zorgt ervoor dat er bij scene load niets kan evalueren
        director.playableAsset = null;
    }

    public void PlayCutscene(PlayableAsset timeline)
    {
        if (timeline == null)
        {
            Debug.LogWarning("PlayCutscene called with null timeline", this);
            return;
        }

        director.Stop();
        director.playableAsset = timeline;
        director.time = 0;
        director.Play();
    }

    public void StopCutscene()
    {
        director.Stop();
        director.playableAsset = null;
    }
}
