using UnityEngine;
using UnityEngine.UI;

public class KettleFill : MonoBehaviour
{
    public float fillAmount = 0f;
    public float maxFill = 1f;
    public float fillSpeed = 0.25f;
    public Slider fillSlider;

    [HideInInspector]
    public bool isFilling = false;

    [Header("Sound Effects")]
    public AudioSource audioPouringWater;      // Wasser läuft ein
    public AudioSource audioBoilingWater;      // Wasser kocht
    public AudioSource audioWhistlingKettle;   // Teekessel-Pfeifen
    public AudioSource audioSpillingWater;     // Wasser überläuft
    public AudioSource audioVoiceOverSpill;    // Voice-Over beim Überlaufen

    private bool hasSpilled = false;  // Überlauf einmalig triggern
    private bool hasStartedBoiling = false; // Kochen + Pfeifen einmalig triggern

    void Update()
    {
        // ---------------------------------------------------------
        // EINFLIESSEN / POURING
        // ---------------------------------------------------------
        if (isFilling)
        {
            if (audioPouringWater != null && !audioPouringWater.isPlaying)
                audioPouringWater.Play();
            
            // Während Einfüllen darf Kochen & Pfeifen NICHT laufen
            StopBoilingAndWhistle();

            if (fillAmount < maxFill)
            {
                fillAmount += fillSpeed * Time.deltaTime;
            }
            else
            {
                // Überlauf beginnt
                if (!hasSpilled)
                {
                    hasSpilled = true;

                    if (audioSpillingWater != null)
                        audioSpillingWater.Play();

                    if (audioVoiceOverSpill != null)
                        audioVoiceOverSpill.Play();
                }
            }

            fillSlider.value = Mathf.Clamp01(fillAmount / maxFill);
        }
        else
        {
            // Wenn der Hahn nicht läuft → Pouring stoppen
            if (audioPouringWater != null && audioPouringWater.isPlaying)
                audioPouringWater.Stop();
        }

        // ---------------------------------------------------------
        // KOCHEN & PFEIFEN
        // nur wenn der Kessel NICHT gefüllt wird und VOLL ist
        // ---------------------------------------------------------
        if (!isFilling && fillAmount >= maxFill)
        {
            if (!hasStartedBoiling)
            {
                hasStartedBoiling = true;

                if (audioBoilingWater != null)
                    audioBoilingWater.Play();

                if (audioWhistlingKettle != null)
                    audioWhistlingKettle.Play();
            }
        }
        else
        {
            StopBoilingAndWhistle();
        }
    }

    // ---------------------------------------------------------
    // Öffentliche Funktionen
    // ---------------------------------------------------------
    public void StartFilling()
    {
        isFilling = true;
        hasSpilled = false; // Überlauf resetten
    }

    public void StopFilling()
    {
        isFilling = false;
    }

    // ---------------------------------------------------------
    // Hilfsfunktion
    // ---------------------------------------------------------
    private void StopBoilingAndWhistle()
    {
        hasStartedBoiling = false;

        if (audioBoilingWater != null && audioBoilingWater.isPlaying)
            audioBoilingWater.Stop();

        if (audioWhistlingKettle != null && audioWhistlingKettle.isPlaying)
            audioWhistlingKettle.Stop();
    }
}
