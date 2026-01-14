using UnityEngine;
using UnityEngine.Playables;

public class TimelineRigActivator : MonoBehaviour
{
    [SerializeField] private GameObject rigObject;
    [SerializeField] private PlayableDirector director;

    void Start()
    {
        rigObject.SetActive(false);
        director.played += OnPlayed;
        director.stopped += OnStopped;
    }

    void OnPlayed(PlayableDirector d)
    {
        rigObject.SetActive(true);
    }

    void OnStopped(PlayableDirector d)
    {
        rigObject.SetActive(false);
    }
}
