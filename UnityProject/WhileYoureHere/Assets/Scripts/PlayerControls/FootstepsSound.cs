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
                    if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out var hit, 2f))
                    {
                        Terrain terrain = hit.collider.GetComponent<Terrain>();
                        if (terrain != null)
                        {
                            Vector3 terrainPos = hit.point - terrain.transform.position;
                            TerrainData tData = terrain.terrainData;
                            int mapX = (int)((terrainPos.x / tData.size.x) * tData.alphamapWidth);
                            int mapZ = (int)((terrainPos.z / tData.size.z) * tData.alphamapHeight);

                            float[,,] splat = tData.GetAlphamaps(mapX, mapZ, 1, 1);

                            float max = 0;
                            int index = 0;
                            for (int i = 0; i < splat.GetLength(2); i++)
                            {
                                if (splat[0, 0, i] > max)
                                {
                                    max = splat[0, 0, i];
                                    index = i;
                                }
                            }

                            switch (terrain.terrainData.terrainLayers[index].name)
                            {
                                case "Grass":
                                    audioSource.PlayOneShot(fs.Grass[Random.Range(0, fs.Grass.Length)]);
                                    break;
                                case "Leaves":
                                    audioSource.PlayOneShot(fs.Leaves[Random.Range(0, fs.Leaves.Length)]);
                                    break;
                                case "Snow":
                                    audioSource.PlayOneShot(fs.Snow[Random.Range(0, fs.Snow.Length)]);
                                    break;
                                case "Stone":
                                    audioSource.PlayOneShot(fs.Stone[Random.Range(0, fs.Stone.Length)]);
                                    break;
                                default:
                                    audioSource.PlayOneShot(fs.Floor[Random.Range(0, fs.Floor.Length)]);
                                    break;
                            }
                        }
                        else
                        {
                            switch (hit.collider.tag)
                            {
                                case "GROUND/grass":
                                    audioSource.PlayOneShot(fs.Grass[Random.Range(0, fs.Grass.Length)]);
                                    break;
                                case "GROUND/leaves":
                                    audioSource.PlayOneShot(fs.Leaves[Random.Range(0, fs.Leaves.Length)]);
                                    break;
                                case "GROUND/snow":
                                    audioSource.PlayOneShot(fs.Snow[Random.Range(0, fs.Snow.Length)]);
                                    break;
                                case "GROUND/stone":
                                    audioSource.PlayOneShot(fs.Stone[Random.Range(0, fs.Stone.Length)]);
                                    break;
                                default:
                                    audioSource.PlayOneShot(fs.Floor[Random.Range(0, fs.Floor.Length)]);
                                    break;
                            }
                        }
                    }
                    _time = fs.FootStepOffset;
                }
            }
        }
    }
}
