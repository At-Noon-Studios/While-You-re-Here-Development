using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace starting_screen
{
    public class OptionsMenu : MonoBehaviour
    {
        [Header("Screen Options")] [SerializeField]
        private TMP_Dropdown resolutionDropdown;

        [SerializeField] private Toggle fullScreenToggle;

        [Header("Audio Options")] [SerializeField]
        private AudioMixer mixer;

        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider voiceSlider;
        [SerializeField] private Button resetButton;

        [Header("Text Options")] [SerializeField]
        private TextMeshProUGUI masterLabel;

        [SerializeField] private TextMeshProUGUI musicLabel;
        [SerializeField] private TextMeshProUGUI sfxLabel;
        [SerializeField] private TextMeshProUGUI voiceLabel;

        private Resolution[] allResolutions;
        private bool isFullScreen;
        private int currentResolutionIndex;

        private readonly List<Resolution> selectedResolutions = new();

        // =========================
        // UNITY LIFECYCLE
        // =========================

        private void OnEnable()
        {
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

        // =========================
        // AUDIO
        // =========================

        private void AddSliderListeners()
        {
            masterSlider.onValueChanged.AddListener(v => SetVolume("Master", v, masterLabel));
            musicSlider.onValueChanged.AddListener(v => SetVolume("Music", v, musicLabel));
            sfxSlider.onValueChanged.AddListener(v => SetVolume("SFX", v, sfxLabel));
            voiceSlider.onValueChanged.AddListener(v => SetVolume("Voice", v, voiceLabel));
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
            masterSlider.value = PlayerPrefs.GetFloat("Master", 1f);
            musicSlider.value = PlayerPrefs.GetFloat("Music", 1f);
            sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1f);
            voiceSlider.value = PlayerPrefs.GetFloat("Voice", 1f);
        }

        private void ApplyInitialVolumes()
        {
            SetVolume("Master", masterSlider.value, masterLabel);
            SetVolume("Music", musicSlider.value, musicLabel);
            SetVolume("SFX", sfxSlider.value, sfxLabel);
            SetVolume("Voice", voiceSlider.value, voiceLabel);
        }

        private void SetVolume(string parameter, float sliderValue, TextMeshProUGUI label)
        {
            // Convert 0–100 slider to 0–1 normalized
            var normalized = sliderValue / 100f;

            // Full mute if 0
            var volumeDb = normalized <= 0f
                ? -80f
                : Mathf.Log10(normalized) * 20f;

            // Apply to AudioMixer
            if (!mixer.SetFloat(parameter, volumeDb))
            {
                Debug.LogWarning($"AudioMixer parameter '{parameter}' not found!");
            }

            // Update text
            label.text = sliderValue + "%";

            // Save
            PlayerPrefs.SetFloat(parameter, normalized);
        }


        private void ResetVolumes()
        {
            masterSlider.value = 1f;
            musicSlider.value = 1f;
            sfxSlider.value = 1f;
            voiceSlider.value = 1f;
        }

        // =========================
        // SCREEN OPTIONS
        // =========================

        private void SetupResolutionDropdown()
        {
            allResolutions = Screen.resolutions;
            var options = new List<string>();

            foreach (var res in allResolutions)
            {
                var option = $"{res.width} x {res.height}";
                if (!options.Contains(option))
                {
                    options.Add(option);
                    selectedResolutions.Add(res);
                }
            }

            options.Reverse();
            selectedResolutions.Reverse();

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);

            currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            ApplyResolution();
        }

        private void SetupFullscreenToggle()
        {
            isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
            fullScreenToggle.isOn = isFullScreen;
        }

        public void ChangeResolution()
        {
            currentResolutionIndex = resolutionDropdown.value;
            ApplyResolution();
            PlayerPrefs.SetInt("ResolutionIndex", currentResolutionIndex);
        }

        public void ChangeFullScreen()
        {
            isFullScreen = fullScreenToggle.isOn;
            Screen.fullScreen = isFullScreen;
            PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
        }

        private void ApplyResolution()
        {
            var res = selectedResolutions[currentResolutionIndex];
            Screen.SetResolution(res.width, res.height, isFullScreen);
        }
    }
}