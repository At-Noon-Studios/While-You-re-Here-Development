using UnityEngine;

public class HeatKettle : MonoBehaviour
{
    public ParticleSystem boilingParticles;   // Dampf / Kochen
    public float requiredFill = 0.2f;         // Mindestfüllstand
    public float heatTime = 3f;               // Zeit bis Kochen beginnt

    private float heatTimer = 0f;

    [Header("Sound Effects")]
    public AudioSource audioBoilingWater;     // Kochen
    public AudioSource audioWhistlingKettle;  // Pfeifen

    void OnTriggerStay(Collider other)
    {
        // KettleFill befindet sich auf dem Parent!
        KettleFill kettle = other.GetComponentInParent<KettleFill>();

        if (kettle != null && kettle.fillAmount >= requiredFill)
        {
            heatTimer += Time.deltaTime;

            if (heatTimer >= heatTime)
            {
                // Partikel → Kochen sichtbar
                if (!boilingParticles.isPlaying)
                    boilingParticles.Play();

                // 🔊 BOILING SOUND
                if (audioBoilingWater != null && !audioBoilingWater.isPlaying)
                    audioBoilingWater.Play();

                // 🔊 WHISTLING SOUND
                if (audioWhistlingKettle != null && !audioWhistlingKettle.isPlaying)
                    audioWhistlingKettle.Play();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Sobald der Kessel heruntergenommen wird:
        if (other.GetComponentInParent<KettleFill>() != null)
        {
            heatTimer = 0f;

            // Partikel stoppen
            boilingParticles.Stop();

            // Sounds stoppen
            if (audioBoilingWater != null && audioBoilingWater.isPlaying)
                audioBoilingWater.Stop();

            if (audioWhistlingKettle != null && audioWhistlingKettle.isPlaying)
                audioWhistlingKettle.Stop();
        }
    }
}