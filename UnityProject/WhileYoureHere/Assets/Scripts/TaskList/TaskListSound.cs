using UnityEngine;

public class TaskListSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip taskListGrabSound;
    [SerializeField] private AudioClip taskListCloseSound;

    public void PlayTasklistGrabSound()
    {
        _audioSource.PlayOneShot(taskListGrabSound);
    }

    public void PlayTasklistCloseSound()
    {
        _audioSource.PlayOneShot(taskListCloseSound);
    }
}