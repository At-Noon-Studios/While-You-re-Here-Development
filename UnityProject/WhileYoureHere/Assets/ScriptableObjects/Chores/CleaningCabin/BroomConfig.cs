using UnityEngine;

[CreateAssetMenu(fileName = "BroomConfig", menuName = "ScriptableObjects/CleaningCabin/BroomConfig")]
public class BroomConfig : ScriptableObject
{
    [Header("Broom Max Position While Sweeping")]
    [SerializeField] private float minBroomXPos = -3f;
    [SerializeField] private float maxBroomXPos = 3f;
    
    [Header("Sweeping Area Color Transition Speed")]
    [SerializeField] private float broomSpeed = 0.004f;
    [SerializeField] private float lerpSpeed = 1.0f;
    
    [Header("Sound that should play while sweeping")]
    [SerializeField] private AudioClip sweepingAudioClip;
    
    public float MinBroomXPos => minBroomXPos;
    public float MaxBroomXPos => maxBroomXPos;
    public float BroomSpeed => broomSpeed;
    public float LerpSpeed => lerpSpeed;
    public AudioClip SweepingAudioClip => sweepingAudioClip;
}
