using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace time
{
    public class TimeManager : MonoBehaviour
    {
        private Light _sunLight;

        [Header("Current Time")] 
        [Range(1, 8)] [SerializeField] private int days;
        [Range(0, 23)] [SerializeField] private int hours;

        [Header("Transitions")] [SerializeField]
        private List<TimeTransition> transitions = new List<TimeTransition>();

        private int _lastDay;
        private int _lastHour;

        private static readonly int Texture1 = Shader.PropertyToID("_Texture1");
        private static readonly int Texture2 = Shader.PropertyToID("_Texture2");
        private static readonly int Blend = Shader.PropertyToID("_Blend");

        [SerializeField] private Light[] cabinPointLights;
        [SerializeField] private Light[] cabinSpotLights;

        private void Awake()
        {
            _sunLight = GameObject.FindWithTag("Sun").GetComponent<Light>();
        }

        public void ChangeTime(int day, int hour)
        {
            if (day == _lastDay && hour == _lastHour) return;
            days = day;
            hours = hour;
            Validate();
        }

        private void Validate()
        {
            if (days == _lastDay && hours == _lastHour) return;
            _lastDay = days;
            _lastHour = hours;
            TryStartTransition(days, hours);
        }

        private void TryStartTransition(int day, int hour)
        {
            foreach (var transition in
                     transitions.Where(transition => transition.day == day && transition.hour == hour))
            {
                Debug.Log($"Starting transition for Day {day}, Hour {hour}");
                StartCoroutine(LerpSkybox(transition.fromSkybox, transition.toSkybox, transition.duration));
                StartCoroutine(LerpLight(transition.lightGradient, transition.duration));
                StartCoroutine(LerpSunRotation(transition.startSunRotation, transition.endSunRotation,
                    transition.duration));
                StartCoroutine(LerpCabinLights(transition.endPointLightIntensity, transition.endSpotLightIntensity, transition.duration));
                return;
            }
            Debug.Log($"No transition defined for Day {day}, Hour {hour}");
        }

        private static IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
        {
            if (!a || !b) yield break;

            RenderSettings.skybox.SetTexture(Texture1, a);
            RenderSettings.skybox.SetTexture(Texture2, b);
            RenderSettings.skybox.SetFloat(Blend, 0);

            for (float i = 0; i < time; i += Time.deltaTime)
            {
                RenderSettings.skybox.SetFloat(Blend, i / time);
                yield return null;
            }

            RenderSettings.skybox.SetTexture(Texture1, b);
        }

        private IEnumerator LerpLight(Gradient lightGradient, float time)
        {
            if (lightGradient == null) yield break;

            for (float i = 0; i < time; i += Time.deltaTime)
            {
                _sunLight.color = lightGradient.Evaluate(i / time);
                RenderSettings.fogColor = _sunLight.color;
                yield return null;
            }
        }

        private IEnumerator LerpSunRotation(float startAngle, float endAngle, float time)
        {
            var initialRotation = _sunLight.transform.rotation.eulerAngles;

            for (float t = 0; t < time; t += Time.deltaTime)
            {
                var angle = Mathf.Lerp(startAngle, endAngle, t / time);
                var newRotation = new Vector3(angle, initialRotation.y, initialRotation.z);
                _sunLight.transform.rotation = Quaternion.Euler(newRotation);
                yield return null;
            }

            _sunLight.transform.rotation = Quaternion.Euler(endAngle, initialRotation.y, initialRotation.z);
        }
        
        //update en test zijn enkel voor pull-request doeleinden 
        void Update()
        {
            Test();
        }
        void Test()
        {
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                TryStartTransition(1, 16);
            }
            if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                TryStartTransition(1, 6);
            }
        }
        
        private IEnumerator LerpCabinLights(float endPointLightIntensity, float endSpotLightIntensity,
            float transitionDuration)
        {
            if (cabinPointLights == null || cabinPointLights.Length == 0 || cabinSpotLights == null || cabinSpotLights.Length == 0) yield break;
            
            float[] startPointIntensity = cabinPointLights.Select(pointLight => pointLight.intensity).ToArray();
            float[] startSpotIntensity = cabinSpotLights.Select(spotLight => spotLight.intensity).ToArray();
            
            for (float t = 0; t < transitionDuration; t += Time.deltaTime) 
            {
                for (var i = 0; i < cabinPointLights.Length; i++)
                {
                    cabinPointLights[i].intensity = Mathf.Lerp(startPointIntensity[i], endPointLightIntensity, t / transitionDuration);
                }
                for (var i = 0; i < cabinSpotLights.Length; i++)
                {
                    cabinSpotLights[i].intensity = Mathf.Lerp(startSpotIntensity[i], endSpotLightIntensity, t / transitionDuration);
                }
                yield return null;
            }
        }
    }
}