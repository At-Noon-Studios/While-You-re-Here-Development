using UnityEngine;

namespace ScriptableObjects.FootSteps
{
    [CreateAssetMenu(fileName = "FootStepsSO", menuName = "ScriptableObjects/FootStepsSO")]
    public class FootStepsData : ScriptableObject
    {
        [Header("Footstep Sound Effects")]
        [Space]
        [SerializeField] private AudioClip[] leaves;
        [SerializeField] private AudioClip[] snow;
        [SerializeField] private AudioClip[] grass;
        [SerializeField] private AudioClip[] path;
        [SerializeField] private AudioClip[] floor;

        [Header("Interval between each footstep sound")]
        [Space]
        [SerializeField] float footstepOffset = 0.5f;

        public AudioClip[] Leaves => leaves;
        public AudioClip[] Snow => snow;
        public AudioClip[] Grass => grass;
        public AudioClip[] Path => path;
        public AudioClip[] Floor => floor;
        public float FootStepOffset => footstepOffset;
    }
}