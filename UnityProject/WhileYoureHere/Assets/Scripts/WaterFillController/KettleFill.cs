using UnityEngine;

namespace WaterFillController
{
    public class KettleFill : MonoBehaviour
    {
        public float fillAmount = 0f;
        public float maxFill = 1f;
        public float fillSpeed = 0.25f;

        [HideInInspector]
        public bool isFilling = false;

        void Update()
        {
            if (!isFilling || !(fillAmount < maxFill)) return;
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp(fillAmount, 0f, maxFill);
        }

        public void StartFilling()  => isFilling = true;
        public void StopFilling()   => isFilling = false;
    }
}