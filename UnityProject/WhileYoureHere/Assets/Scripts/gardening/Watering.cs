using UnityEngine;

namespace gardening
{
    public class Watering : MonoBehaviour
    {
        [SerializeField] private ParticleSystem waterParticleSystem;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            waterParticleSystem = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Angle(Vector3.down, transform.forward) <= 90f)
            {
                waterParticleSystem.Play();
            }
            else
            {
                waterParticleSystem.Stop();
            }
        }
    }
}
