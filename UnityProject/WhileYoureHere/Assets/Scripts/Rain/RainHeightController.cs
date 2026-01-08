using UnityEngine;

namespace Rain
{
    using UnityEngine;

    public class RainHeightController : MonoBehaviour
    {
        [Header("Regen Particles")]
        [SerializeField] private ParticleSystem rain;

        [Header("Indoor Volume")]
        [SerializeField] private Collider[] indoorVolumes;

        [Header("Indoor Partikelhöhe")]
        [SerializeField] private float indoorHeight = 2f;

        private ParticleSystem.ShapeModule _shape;
        private float _outdoorHeight;

        void Awake()
        {
            _shape = rain.shape;
            _outdoorHeight = _shape.scale.y; // Originalhöhe merken
        }

        void Update()
        {
            Vector3 playerPos = transform.position;
            bool inside = false;

            foreach (var volume in indoorVolumes)
            {
                if (volume.bounds.Contains(playerPos))
                {
                    inside = true;
                    break;
                }
            }

            _shape.scale = new Vector3(_shape.scale.x, inside ? indoorHeight : _outdoorHeight, _shape.scale.z);
        }
    }

}
