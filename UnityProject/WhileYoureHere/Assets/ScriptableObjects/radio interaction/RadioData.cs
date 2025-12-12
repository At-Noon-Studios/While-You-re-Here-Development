using UnityEngine;

[CreateAssetMenu(fileName = "RadioData", menuName = "ScriptableObjects/RadioData")]
public class RadioData : ScriptableObject
{
    [Header("Radio settings")] public int tuningWaitTime;
    public float sensitivity = 0.02f;
    private const float _sliderSensitivity = 0.0002f;
    private const float _maxSliderPos = 0.05f;
    private const float _minSliderPos = -0.05f;

    public float MinSliderPos() => _minSliderPos;
    public float MaxSliderPos() => _maxSliderPos;
    public float SliderSensitivity() => _sliderSensitivity;
}