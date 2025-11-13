using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MovementController))]
public class MovementTest : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip[] leaves;
    [SerializeField] AudioClip[] snow;
    [SerializeField] AudioClip[] grass;
    [SerializeField] AudioClip[] floor;


    int layerMask = 1 << 6;

    public float time;
    public float footstepOffset = 0.5f;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    void Update()
    {
        HandleFootsteps();
    }

    void HandleFootsteps()
    {
        time -= Time.deltaTime;
        if (time < 0.0f)
        {
            if (Physics.Raycast(transform.position, -Vector3.down, out RaycastHit hit, 4.0f, layerMask))
            {
                Debug.Log("I be hitting sumn: " + hit.collider.name);
                Debug.DrawRay(transform.position, -Vector3.down, Color.red);
                switch (hit.collider.tag)
                {
                    case "Player":
                        _audioSource.PlayOneShot(snow[Random.Range(0, snow.Length - 1)]);
                        break;
                    case "test1":
                        Debug.Log("something");
                        break;
                    default:
                        Debug.Log("something");
                        break;
                }
            }
            time = footstepOffset;
        }
    }
}
