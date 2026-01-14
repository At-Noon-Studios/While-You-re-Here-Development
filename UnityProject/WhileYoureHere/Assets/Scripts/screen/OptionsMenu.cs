using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace screen
{
    public class OptionsMenu : MonoBehaviour
    {
        [Header("Screen Options")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullScreenToggle;

        [Header("Audio Options")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider voiceSlider;
        [SerializeField] private Button resetButton;

        [Header("Text Options")]
        [SerializeField] private TextMeshProUGUI masterLabel;
        [SerializeField] private TextMeshProUGUI musicLabel;
        [SerializeField] private TextMeshProUGUI sfxLabel;
        [SerializeField] private TextMeshProUGUI voiceLabel;

        private Resolution[] _allResolutions;
        private bool _isFullScreen;
        private int _currentResolutionIndex;
        private readonly List<Resolution> _selectedResolutions = new();
        
        private void OnEnable()
        {
            // Sync fullscreen toggle with actual screen mode
            _isFullScreen = Screen.fullScreen;
            fullScreenToggle.isOn = _isFullScreen;

            // Optionally: sync resolution dropdown too
            SetupResolutionDropdown();
            
            RemoveSliderListeners();
            LoadSavedVolumes();
            ApplyInitialVolumes();
            AddSliderListeners();
        }

        private void Start()
        {
            SetupResolutionDropdown();
            SetupFullscreenToggle();
            resetButton.onClick.AddListener(ResetVolumes);
        }
        
        // AUDIO
        private void AddSliderListeners()
        {
            masterSlider.onValueChanged.AddListener(v => SetMasterVolume((int)v));
            musicSlider.onValueChanged.AddListener(v => SetMusicVolume((int)v));
            sfxSlider.onValueChanged.AddListener(v => SetSfxVolume((int)v));
            voiceSlider.onValueChanged.AddListener(v => SetVoiceVolume((int)v));
        }

        private void RemoveSliderListeners()
        {
            masterSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.RemoveAllListeners();
            voiceSlider.onValueChanged.RemoveAllListeners();
        }

        private void LoadSavedVolumes()
        {
            masterSlider.value = PlayerPrefs.GetFloat("Master", 1f) * 100f;
            musicSlider.value = PlayerPrefs.GetFloat("Music", 1f) * 100f;
            sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1f) * 100f;
            voiceSlider.value = PlayerPrefs.GetFloat("Voice", 1f) * 100f;
        }

        public void SetMasterVolume(int sliderValue) => SetVolume("Master", sliderValue, masterLabel);
        public void SetMusicVolume(int sliderValue) => SetVolume("Music", sliderValue, musicLabel);
        public void SetSfxVolume(int sliderValue) => SetVolume("SFX", sliderValue, sfxLabel);
        public void SetVoiceVolume(int sliderValue) => SetVolume("Voice", sliderValue, voiceLabel);

        private void ApplyInitialVolumes()
        {
            SetMasterVolume((int)masterSlider.value);
            SetMusicVolume((int)musicSlider.value);
            SetSfxVolume((int)sfxSlider.value);
            SetVoiceVolume((int)voiceSlider.value);
        }

        private void SetVolume(string parameter, int sliderValue, TextMeshProUGUI label)
        {
            var normalized = Mathf.Clamp(sliderValue, 0, 100) / 100f;

            var volumeDb = (normalized <= 0f) ? -80f : Mathf.Log10(normalized) * 20f;

            mixer.SetFloat(parameter, volumeDb);

            label.text = sliderValue + "%";

            PlayerPrefs.SetFloat(parameter, normalized);
        }

        private void ResetVolumes()
        {
            masterSlider.value = 100;
            musicSlider.value = 100;
            sfxSlider.value = 100;
            voiceSlider.value = 100;

            SetMasterVolume(100);
            SetMusicVolume(100);
            SetSfxVolume(100);
            SetVoiceVolume(100);
        }
        
        // SCREEN OPTIONS
        private void SetupResolutionDropdown()
        {
            _allResolutions = Screen.resolutions;
            var options = new List<string>();

            foreach (var res in _allResolutions)
            {
                var option = $"{res.width} x {res.height}";
                if (!options.Contains(option))
                {
                    options.Add(option);
                    _selectedResolutions.Add(res);
                }
            }

            options.Reverse();
            _selectedResolutions.Reverse();

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);

            _currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
            resolutionDropdown.value = _currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            ApplyResolution();
        }

        private void SetupFullscreenToggle()
        {
            _isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
            fullScreenToggle.isOn = _isFullScreen;
        }

        public void ChangeResolution()
        {
            _currentResolutionIndex = resolutionDropdown.value;
            ApplyResolution();
            PlayerPrefs.SetInt("ResolutionIndex", _currentResolutionIndex);
        }

        public void ChangeFullScreen()
        {
            _isFullScreen = fullScreenToggle.isOn;
            Screen.fullScreen = _isFullScreen;
            PlayerPrefs.SetInt("FullScreen", _isFullScreen ? 1 : 0);
        }

        private void ApplyResolution()
        {
            var res = _selectedResolutions[_currentResolutionIndex];
            Screen.SetResolution(res.width, res.height, _isFullScreen);
        }
        
        //SUBTITLES
        // Future implementation for subtitle options can be added here
    }
}