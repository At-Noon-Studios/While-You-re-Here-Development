using making_tea;
using UnityEngine;

namespace gardening
{
    public class Watering : MonoBehaviour
    {
        [Header("Water Particles")]
        [SerializeField] private ParticleSystem waterParticleSystem;
        
        [Header("Dependencies")]
        [SerializeField] private KettleFill wateringCanFill;
        
        [Header("Sound")]
        [SerializeField] private AudioSource pouringSound;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            pouringSound = GetComponent<AudioSource>();
            
            if (wateringCanFill == null)
                wateringCanFill = GetComponent<KettleFill>();
        }

        // Update is called once per frame
        void Update()
        {
            if (wateringCanFill == null || waterParticleSystem == null)
                return;
            
            // don't start if empty
            if (wateringCanFill.fillAmount <= 0f)
            {
                waterParticleSystem.Stop();
                
                if (pouringSound.isPlaying)
                    pouringSound.Stop();
                return;
            }

            if (Vector3.Angle(Vector3.down, transform.forward) <= 90f)
            {
                waterParticleSystem.Play();
                
                if (!pouringSound.isPlaying)
                    pouringSound.Play();
            }
            else
            {
                waterParticleSystem.Stop();
                
                if (pouringSound.isPlaying)
                    pouringSound.Stop();
            }
        }
    }
}
