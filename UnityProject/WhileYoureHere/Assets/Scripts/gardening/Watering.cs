using System.Collections;
using chore;
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
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (wateringCanFill == null)
                wateringCanFill = GetComponent<KettleFill>();
                
            if (waterParticleSystem == null)
                waterParticleSystem = GetComponent<ParticleSystem>();
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
                return;
            }
                
            if(Vector3.Angle(Vector3.down, transform.forward) <= 90f)
                waterParticleSystem.Play();
            else
                waterParticleSystem.Stop();
        }
    }
}
