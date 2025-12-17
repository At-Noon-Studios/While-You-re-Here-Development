using UnityEngine;

[CreateAssetMenu(fileName = "SweepingAreaConfig", menuName = "ScriptableObjects/CleaningCabin/SweepingAreaConfig")]
public class SweepingAreaConfig : ScriptableObject
{
    [Header("Sound that plays when the area is destroyed")]
    [SerializeField] private AudioClip destroyAreaClip;
    
    [Header("How fast the minigame transitions to the starting position")]
    [Range(3.0f, 10.0f)]
    [SerializeField] private float transitionSpeed = 3f;
    
    public AudioClip DestroyAreaClip => destroyAreaClip;
    public float TransitionSpeed => transitionSpeed;
}
