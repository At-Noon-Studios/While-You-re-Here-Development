using System.Collections.Generic;
using UnityEngine;

namespace AudioPresetResearch
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPresetComponent : MonoBehaviour
    {
        [HideInInspector] public List<AudioPresetData> presets = new List<AudioPresetData>();
        [HideInInspector] public string newPresetName;
        [HideInInspector] public int selectedPresetIndex = -1;
        private AudioSource audioSource;

        private void OnEnable()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void SavePreset()
        {
            if (!TryGetAudioSource(out var source)) return;
            
            if (string.IsNullOrWhiteSpace(newPresetName) && TryGetSelectedPreset(out var selectedPreset))
            {
                selectedPreset.CaptureFrom(source);
                newPresetName = selectedPreset.presetName;
                return;
            }
            
            if (string.IsNullOrWhiteSpace(newPresetName))
            {
                Debug.LogWarning("Preset name is empty.");
                return;
            }

            string trimmedName = newPresetName.Trim();
            int existingIndex = presets.FindIndex(p => p.presetName == trimmedName);

            if (existingIndex >= 0)
            {
                UpdatePreset(source, existingIndex);
            }
            else
            {
                CreatePreset(source, trimmedName);
            }

            newPresetName = "";
        }

        private void UpdatePreset(AudioSource source, int existingIndex)
        {
            presets[existingIndex].CaptureFrom(source);
            selectedPresetIndex = existingIndex;
        }

        private void CreatePreset(AudioSource source, string trimmedName)
        {
            var preset = new AudioPresetData();
            preset.CaptureFrom(source);
            preset.presetName = trimmedName;

            presets.Add(preset);
            selectedPresetIndex = presets.Count - 1;
        }
        
        public void LoadPreset()
        {
            if (!TryGetAudioSource(out var source)) return;
            if (!TryGetSelectedPreset(out var preset)) return;
            
            preset.ApplyTo(source);
            
            newPresetName = preset.presetName;
        }

        public void DeletePreset()
        {
            presets.RemoveAt(selectedPresetIndex);

            if (presets.Count == 0)
            {
                selectedPresetIndex = -1;
            }
            else
            {
                selectedPresetIndex = Mathf.Clamp(selectedPresetIndex - 1, 0, presets.Count - 1);
            }
        }


        public void PreviewCustomPreset()
        {
            if (!TryGetAudioSource(out var source)) return;
            if (!TryGetSelectedPreset(out var preset)) return;
            preset.ApplyTo(source);
            source.Stop();
            source.Play();
        }

        private bool TryGetAudioSource(out AudioSource source)
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                Debug.LogError("No AudioSource found");
                source = null;
                return false;
            }

            source = audioSource;
            return true;
        }

        private bool TryGetSelectedPreset(out AudioPresetData preset)
        {
            if (selectedPresetIndex < 0 || selectedPresetIndex >= presets.Count)
            {
                Debug.LogWarning("No valid preset selected");
                preset = null;
                return false;
            }

            preset = presets[selectedPresetIndex];
            return true;
        }
    }
}