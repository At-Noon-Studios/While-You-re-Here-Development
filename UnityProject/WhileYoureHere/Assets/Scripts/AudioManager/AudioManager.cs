using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource soundObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySound(AudioClip audioClip, Transform position, float volume)
    {
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float audioClipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, audioClipLength);
    }
}