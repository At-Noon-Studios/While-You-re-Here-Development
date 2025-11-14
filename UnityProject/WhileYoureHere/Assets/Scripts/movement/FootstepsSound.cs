using System.Numerics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MovementController))]
public class FootstepsSound : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip[] leaves;
    [SerializeField] AudioClip[] snow;
    [SerializeField] AudioClip[] grass;
    [SerializeField] AudioClip[] floor;
    // Camera mainCamera;
    // [SerializeField] Transform raycastHelper;

    [SerializeField] MovementController player;

    [Header("Variables for footstep frequency")]
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
        //int layerMask = 1 << 6;
        //layerMask = ~layerMask;
        time -= Time.deltaTime;

        if (time < 0.0f)
        {
            if (Physics.Raycast(transform.position + Vector3.up * .01f, Vector3.down, out var hit, 1f))
            {
                //Debug.Log("I am hitting: " + hit.collider.name);
                //Debug.DrawRay(transform.position, Vector3.down * .01f, Color.red, 60);
                switch (hit.collider.tag)
                {
                    case "GROUND/snow":
                        _audioSource.PlayOneShot(snow[Random.Range(0, snow.Length - 1)]);
                        break;
                    case "GROUND/leaves":
                        _audioSource.PlayOneShot(leaves[Random.Range(0, leaves.Length - 1)]);
                        break;
                    case "GROUND/floor":
                        _audioSource.PlayOneShot(floor[Random.Range(0, floor.Length - 1)]);
                        break;
                    case "GROUND/grass":
                        _audioSource.PlayOneShot(grass[Random.Range(0, grass.Length - 1)]);
                        break;
                    default:
                        _audioSource.PlayOneShot(floor[Random.Range(0, floor.Length - 1)]);
                        break;
                }
            }
            time = footstepOffset;
        }
    }
}