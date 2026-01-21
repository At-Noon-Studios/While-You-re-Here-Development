using UnityEngine;
using UnityEngine.Playables;

namespace cutscenes
{
    public class TimelineChain : MonoBehaviour
    {
        [SerializeField] private PlayableDirector currentTimeline;
        [SerializeField] private PlayableDirector nextTimeline;

        void Start()
        {
            if (currentTimeline != null)
                currentTimeline.stopped += OnTimelineStopped;
        }

        void OnTimelineStopped(PlayableDirector director)
        {
            if (director == currentTimeline && nextTimeline != null)
            {
                nextTimeline.Play();
            }
        }

    }
}
