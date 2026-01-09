// using System.Collections;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class LightScript : MonoBehaviour
// {
//     private Light _light;
//     public float currentIntensity;
//     public int endingIntensity;
//
//     private void Awake()
//     {
//         _light = GetComponent<Light>();
//     }
//
//     private void Update()
//     {
//         if (Keyboard.current.gKey.isPressed)
//         {
//             StartCoroutine(LerpLights());
//         }
//     }
//     private IEnumerator LerpLights()
//     {
//         currentIntensity = _light.intensity;
//         _light.intensity = Mathf.Lerp(currentIntensity, endingIntensity, Time.deltaTime);
//         yield return null;
//     }
// }