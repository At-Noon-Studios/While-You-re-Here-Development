using UnityEngine;

[CreateAssetMenu(fileName = "GarbageConfig", menuName = "ScriptableObjects/CleaningCabin/GarbageConfig")]
public class GarbageConfig : ScriptableObject
{
    [Header("Sound that should play while destroying the garbage")]
    [SerializeField] private AudioClip garbageCollectClip;

    public AudioClip GarbageCollectClip => garbageCollectClip;
}
