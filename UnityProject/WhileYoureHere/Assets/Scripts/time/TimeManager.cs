using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace time
{
    public class TimeManager : MonoBehaviour
    {
        private Light _sunLight;

        [Header("Current Time")] [Range(1, 8)] [SerializeField]
        private int days;

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
                // StartCoroutine(LerpCabinSpotLights(transition.endSpotLightIntensity, transition.duration));
                // StartCoroutine(LerpCabinPointLights(transition.endPointLightIntensity, transition.duration));
                StartCoroutine(LerpCabinLights(transition.endSpotLightIntensity, transition.endPointLightIntensity, transition.duration));
                // transition.startPointLightIntensity, transition.endPointLightIntensity, transition.duration));
                // ChangeLightsIntensity();
                // ChangeSpotLightsIntensity();
                // ChangePointLightsIntensity();
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

        // private void ChangeSpotLightsIntensity()
        // {
        //     foreach (var spotLight in cabinSpotLights)
        //     {
        //         if (days == 1)
        //         {
        //             if (hours >= 6)
        //             {
        //                 spotLight.intensity = 30;
        //             }
        //
        //             if (hours >= 16)
        //             {
        //                 spotLight.intensity = 10;
        //             }
        //
        //             if (hours == 0)
        //             {
        //                 spotLight.intensity = 0;
        //             }
        //         }
        //     }
        // }

        // private void ChangePointLightsIntensity()
        // {
        //     foreach (var pointLight in cabinPointLights)
        //     {
        //         if (days == 1)
        //         {
        //             if (hours >= 6)
        //             {
        //                 pointLight.intensity = 3;
        //             }
        //
        //             if (hours >= 16)
        //             {
        //                 pointLight.intensity = 1;
        //             }
        //
        //             if (hours == 0)
        //             {
        //                 pointLight.intensity = 0;
        //             }
        //         }
        //     }
        // }

        private IEnumerator LerpCabinLights(float endSpotLightIntensity, float endPointLightIntensity, float time)
        {
            for (float t = 0; t < time; t += Time.deltaTime) 
            {
                foreach (var cabinSpotLight in cabinSpotLights)
                {
                    var intensity = cabinSpotLight.intensity;
                    cabinSpotLight.intensity = Mathf.Lerp(intensity, endSpotLightIntensity, Time.deltaTime);
                    yield return null;
                }
                foreach(var cabinPointLight in cabinPointLights)
                {
                    var intensity = cabinPointLight.intensity;
                    cabinPointLight.intensity = Mathf.Lerp(intensity, endPointLightIntensity, Time.deltaTime);
                    yield return null;
                }
            }
        }
        
        // private IEnumerator LerpCabinSpotLights(float endSpotLightIntensity, float time)
        // {
        //     for (float t = 0; t < time; t += Time.deltaTime) 
        //     {
        //         foreach (var cabinSpotLight in cabinSpotLights)
        //         {
        //             var intensity = cabinSpotLight.intensity;
        //             cabinSpotLight.intensity = Mathf.Lerp(intensity, endSpotLightIntensity, Time.deltaTime);
        //             yield return null;
        //         }
        //     }
        // }
        //
        // private IEnumerator LerpCabinPointLights(float endPointLightIntensity, float time)
        // {
        //     for (float t = 0; t < time; t += Time.deltaTime)
        //     {
        //         foreach(var cabinPointLight in cabinPointLights)
        //         {
        //             var intensity = cabinPointLight.intensity;
        //             cabinPointLight.intensity = Mathf.Lerp(intensity, endPointLightIntensity, Time.deltaTime);
        //             yield return null;
        //         }
        //     }
        // }
    }
}