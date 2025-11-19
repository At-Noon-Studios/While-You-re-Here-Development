using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class RadioController : MonoBehaviour
{
    [SerializeField] private RadioTracks[] radioTracks;
    [SerializeField] private AudioClip staticClip;
    // [SerializeField] private Slider tuneSlider;
    // [SerializeField] private Button startButton;
    [SerializeField] private Transform startButtonLocation;
    [SerializeField] private Transform sliderLocation;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask dialLayer;
    [SerializeField] private LayerMask buttonLayer;
    private AudioSource audioSource;
    private int currentRadioIndex = -1;
    private float tuneThreshold = 0.1f;
    private bool radioOn;
    private bool previousRadioState;
    private bool isDragging;
    private float tuneValue;
    private Vector3 startMousePos;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        

    }

    private void Update()
    {

        if (radioOn)
        {
            TuneRadio(tuneValue);
        }

        // Detect when radio was turned off
        if (!radioOn && previousRadioState)
        {
            Debug.Log("Radio turned OFF — stopping audio.");
            audioSource.Stop();
            currentRadioIndex = -1;
        }

        previousRadioState = radioOn;
    }
    
    
   
    public void TurnRadioOn()
    {
        radioOn=!radioOn;
    }

    private void TuneRadio(float value)
    {
         // value = tuneSlider.value;
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
        if (audioSource.clip == clip && audioSource.isPlaying) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
}