using UnityEngine;

[CreateAssetMenu(fileName = "SweepingData", menuName = "ScriptableObjects/SweepingArea")]
public class SweepingData : ScriptableObject
{
    [Header("Sweeping clip sound effect")]
    [SerializeField] private AudioClip sweepingClip;

    public AudioClip SweepingClip => sweepingClip;
}
