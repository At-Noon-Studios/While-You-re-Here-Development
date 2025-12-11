using UnityEngine;
using UnityEngine.UI;

namespace starting_screen
{
    public class OptionsMenu : MonoBehaviour
    {
        [Range(0, 100)]
        private int _brightnessRange;
        private int _brightnessPercentage;
        private int _brightnessDefault;
        
        [Header("Brightness Slider")]
        [SerializeField] private Slider brightnessSlider;
        
        
        private void Start()
        {
            _brightnessDefault = 50;
        }

        public void OnChangeBrightness(float brightness)
        {
            RenderSettings.ambientLight = Color.white * (brightness / 100f);
            Debug.Log($"Brightness changed to: {brightness}");
        }
    }
}