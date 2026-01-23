using UnityEditor;
using UnityEngine;

namespace AudioPresetResearch
{
    [CustomEditor(typeof(AudioPresetComponent))]
    public class AudioPresetEditor : Editor
    {
        private AudioPresetComponent presetComponent;

        private void OnEnable()
        {
            presetComponent = (AudioPresetComponent)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preset Name", EditorStyles.miniBoldLabel);

            EditorGUILayout.BeginHorizontal();
            presetComponent.newPresetName = EditorGUILayout.TextField(presetComponent.newPresetName);
            ShowSavePresetButton();
            ShowPreviewPresetButton();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Available Presets", EditorStyles.miniBoldLabel);

            EditorGUILayout.BeginHorizontal();
            AvailablePresetArray();
            ShowLoadPresetButton();
            ShowDeletePresetButton();
            EditorGUILayout.EndHorizontal();
        }


        private void AvailablePresetArray()
        {
            if (presetComponent.presets.Count == 0)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Popup(0, new[] { "None" });
                EditorGUI.EndDisabledGroup();

                presetComponent.selectedPresetIndex = -1;
                return;
            }

            if (presetComponent.selectedPresetIndex < 0) presetComponent.selectedPresetIndex = 0;
            
            string[] presetNames = new string[presetComponent.presets.Count];
            for (int i = 0; i < presetNames.Length; i++)
            {
                presetNames[i] = presetComponent.presets[i].presetName;
            }

            presetComponent.selectedPresetIndex =
                EditorGUILayout.Popup(presetComponent.selectedPresetIndex, presetNames);
        }

        private void ShowSavePresetButton()
        {
            if (GUILayout.Button("Save Preset", GUILayout.Width(150)))
            {
                presetComponent.SavePreset();
                EditorUtility.SetDirty(presetComponent);
            }
        }

        private void ShowLoadPresetButton()
        {
            GUI.enabled = presetComponent.presets.Count > 0;

            if (GUILayout.Button("Load Preset"))
            {
                presetComponent.LoadPreset();
            }

            GUI.enabled = true;
        }

        private void ShowDeletePresetButton()
        {
            GUI.enabled = presetComponent.presets.Count > 0;

            if (GUILayout.Button("Delete Preset"))
            {
                presetComponent.DeletePreset();
            }

            GUI.enabled = true;
        }

        private void ShowPreviewPresetButton()
        {
            GUI.enabled = presetComponent.presets.Count > 0;

            if (GUILayout.Button("▶ Preview", GUILayout.Width(90)))
            {
                presetComponent.PreviewCustomPreset();
            }

            GUI.enabled = true;
        }
    }
}