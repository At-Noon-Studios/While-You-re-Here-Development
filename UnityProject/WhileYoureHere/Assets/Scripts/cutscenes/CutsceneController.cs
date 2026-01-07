using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    void Awake()
    {
        director.Stop();
        director.time = 0;
        director.Evaluate();
    }

    public void PlayCutscene()
    {
        director.Play();
    }
}
