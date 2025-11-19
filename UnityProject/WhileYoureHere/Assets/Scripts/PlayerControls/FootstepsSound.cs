using PlayerControls;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MovementController))]
public class FootstepsSound : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] FootStepsData fs;

    private MovementController movementController;
    private float time;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        movementController = GetComponent<MovementController>();
    }

    void Update()
    {
        HandleFootsteps();
    }

    void HandleFootsteps()
    {
        time -= Time.deltaTime;
        if (movementController.IsInput)
        {
            if (time < 0.0f)
            {
                if (Physics.Raycast(transform.position + Vector3.up * .01f, Vector3.down, out var hit, 1f))
                {
                    switch (hit.collider.tag)
                    {
                        case "GROUND/snow":
                            _audioSource.PlayOneShot(fs.Snow[Random.Range(0, fs.Snow.Length - 1)]);
                            break;
                        case "GROUND/leaves":
                            _audioSource.PlayOneShot(fs.Leaves[Random.Range(0, fs.Leaves.Length - 1)]);
                            break;
                        case "GROUND/floor":
                            _audioSource.PlayOneShot(fs.Floor[Random.Range(0, fs.Floor.Length - 1)]);
                            break;
                        case "GROUND/grass":
                            _audioSource.PlayOneShot(fs.Grass[Random.Range(0, fs.Grass.Length - 1)]);
                            break;
                        default:
                            _audioSource.PlayOneShot(fs.Floor[Random.Range(0, fs.Floor.Length - 1)]);
                            break;
                    }
                }
                time = fs.FootStepOffset;
            }
        }
    }
}