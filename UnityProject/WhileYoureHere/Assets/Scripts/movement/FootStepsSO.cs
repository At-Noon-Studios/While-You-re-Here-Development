using UnityEngine;

[CreateAssetMenu(fileName = "FootStepsSO", menuName = "Scriptable Objects/FootStepsSO")]
public class FootStepsSO : ScriptableObject
{
    //[SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip[] leaves;
    [SerializeField] AudioClip[] snow;
    [SerializeField] AudioClip[] grass;
    [SerializeField] AudioClip[] floor;

    //public AudioSource FootstepAudioSource => _audioSource;
    public AudioClip[] Leaves => leaves;
    public AudioClip[] Snow => snow;
    public AudioClip[] Grass => grass;
    public AudioClip[] Floor => floor;

}
