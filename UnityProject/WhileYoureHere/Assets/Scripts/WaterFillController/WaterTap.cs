using UnityEngine;

public class WaterTap : MonoBehaviour
{
    public ParticleSystem waterStream;

    [Header("Sound Effects")]
    public AudioSource audioPouringWater;

    [SerializeField]
    private bool _isRunning = false;

    public bool isRunning
    {
        get => _isRunning;
        set
        {
            _isRunning = value;
            UpdateWaterState();
        }
    }

    void Start()
    {
        UpdateWaterState();
    }

    void OnValidate()
    {
        UpdateWaterState();
    }

    public void ToggleTap()
    {
        isRunning = !isRunning;
    }

    private void UpdateWaterState()
    {
        if (waterStream == null) return;

        if (_isRunning)
        {
            waterStream.Play();

            if (audioPouringWater != null && !audioPouringWater.isPlaying)
                audioPouringWater.Play();
        }
        else
        {
            waterStream.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            if (audioPouringWater != null && audioPouringWater.isPlaying)
                audioPouringWater.Stop();
        }
    }
}