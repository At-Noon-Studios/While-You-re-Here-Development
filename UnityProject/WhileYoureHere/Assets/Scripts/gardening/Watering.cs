using UnityEngine;

public class Watering : MonoBehaviour
{
    public ParticleSystem WaterParticleSystem;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WaterParticleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Angle(Vector3.down, transform.forward) <= 90f)
        {
            WaterParticleSystem.Play();
        }
        else
        {
            WaterParticleSystem.Stop();
        }
    }
}
