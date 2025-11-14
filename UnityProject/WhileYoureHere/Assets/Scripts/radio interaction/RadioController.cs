using UnityEngine;
using UnityEngine.UI;

public class RadioController : MonoBehaviour
{
    [SerializeField] private RadioTracks[] radioTracks;
    [SerializeField] private AudioClip staticClip;
    [SerializeField] private Slider tuneSlider;
    [SerializeField] private Button startButton;

    private AudioSource audioSource;
    private int currentRadioIndex = -1;
    private float tuneThreshold = 0.1f;
    private bool radioOn = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startButton.onClick.AddListener(TurnRadioOn);
    }

    private void Update()
    {
        if (radioOn)
        {
            TuneRadio();
        }
    }

    private void TurnRadioOn()
    {
        Debug.Log("Radio On");
        radioOn = true;
    }

    private void TuneRadio()
    {
        var value = tuneSlider.value;
        var stationSpacing = 1f / radioTracks.Length;
        
        var newIndex = Mathf.FloorToInt(value / stationSpacing);
        var stationCenter = newIndex * stationSpacing;

        var isTuned = Mathf.Abs(value - stationCenter) < tuneThreshold;

        switch (isTuned)
        {
            case true when newIndex != currentRadioIndex:
                currentRadioIndex = newIndex;
                Debug.Log("Tuned to station: " + newIndex);
                PlayClip(radioTracks[newIndex].audioClip);
                break;
            case false when audioSource.clip != staticClip:
                currentRadioIndex = -1;
                Debug.Log("Static");
                PlayClip(staticClip);
                break;
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip is null) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
}