using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace starting_screen
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullScreenToggle;
        
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Button resetButton;

        private Resolution[] _allResolutions;
        private bool _isFullScreen;
        private int _currentResolutionIndex;

        private List<Resolution> _selectedResolutionsList;

        private void Start()
        {
            _allResolutions = Screen.resolutions;
            var resolutionOptions = new List<string>();

            foreach (var res in _allResolutions)
            {
                string newRes = res.width + " x " + res.height;
                if (!resolutionOptions.Contains(newRes))
                {
                    resolutionOptions.Add(newRes);
                    _selectedResolutionsList.Add(res);
                }
            }

            resolutionOptions.Reverse();
            _selectedResolutionsList.Reverse();

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutionOptions);

            _currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
            resolutionDropdown.value = _currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            _isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
            fullScreenToggle.isOn = _isFullScreen;

            Screen.SetResolution(
                _selectedResolutionsList[_currentResolutionIndex].width,
                _selectedResolutionsList[_currentResolutionIndex].height,
                _isFullScreen
            );
        }

        public void ChangeResolution()
        {
            _currentResolutionIndex = resolutionDropdown.value;
            Screen.SetResolution(
                _selectedResolutionsList[_currentResolutionIndex].width,
                _selectedResolutionsList[_currentResolutionIndex].height,
                _isFullScreen
            );

            PlayerPrefs.SetInt("ResolutionIndex", _currentResolutionIndex);
            PlayerPrefs.Save();
        }

        public void ChangeFullScreen()
        {
            _isFullScreen = fullScreenToggle.isOn;
            Screen.fullScreen = _isFullScreen;

            PlayerPrefs.SetInt("FullScreen", _isFullScreen ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}