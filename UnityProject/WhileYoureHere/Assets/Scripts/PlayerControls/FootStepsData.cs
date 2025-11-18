using UnityEngine;

[CreateAssetMenu(fileName = "FootStepsSO", menuName = "Scriptable Objects/FootStepsSO")]
public class FootStepsData : ScriptableObject
{
    [Header("Footstep Sound Effects")]
    [Space]
    [SerializeField] AudioClip[] leaves;
    [SerializeField] AudioClip[] snow;
    [SerializeField] AudioClip[] grass;
    [SerializeField] AudioClip[] floor;

    [Header("Interval between each footstep sound")]
    [Space]
    [SerializeField] float footstepOffset = 0.5f;

    public AudioClip[] Leaves => leaves;
    public AudioClip[] Snow => snow;
    public AudioClip[] Grass => grass;
    public AudioClip[] Floor => floor;
    public float FootStepOffset => footstepOffset;
}