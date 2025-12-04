using player_controls;
using ScriptableObjects.FootSteps;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace PlayerControls
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(MovementController))]
    public class FootstepsSound : MonoBehaviour
    {
        [FormerlySerializedAs("_audioSource")] [SerializeField] AudioSource audioSource;
        [SerializeField] FootStepsData fs;

        private MovementController _movementController;
        private float _time;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            _movementController = GetComponent<MovementController>();
        }

        void Update()
        {
            HandleFootsteps();
        }

        void HandleFootsteps()
        {
            _time -= Time.deltaTime;
            if (_movementController.IsInput)
            {
                if (_time < 0.0f)
                {
                    if (Physics.Raycast(transform.position + Vector3.up * .01f, Vector3.down, out var hit, 1f))
                    {
                        switch (hit.collider.tag)
                        {
                            case "GROUND/leaves":
                                audioSource.PlayOneShot(fs.Leaves[Random.Range(0, fs.Leaves.Length - 1)]);
                                break;
                            case "GROUND/snow":
                                audioSource.PlayOneShot(fs.Snow[Random.Range(0, fs.Snow.Length - 1)]);
                                break;
                            case "GROUND/grass":
                                audioSource.PlayOneShot(fs.Grass[Random.Range(0, fs.Grass.Length - 1)]);
                                break;
                            case "GROUND/path":
                                audioSource.PlayOneShot(fs.Path[Random.Range(0, fs.Path.Length - 1)]);
                                break;
                            case "GROUND/floor":
                                audioSource.PlayOneShot(fs.Floor[Random.Range(0, fs.Floor.Length - 1)]);
                                break;
                            default:
                                audioSource.PlayOneShot(fs.Floor[Random.Range(0, fs.Floor.Length - 1)]);
                                break;
                        }
                    }
                    _time = fs.FootStepOffset;
                }
            }
        }
    }
}