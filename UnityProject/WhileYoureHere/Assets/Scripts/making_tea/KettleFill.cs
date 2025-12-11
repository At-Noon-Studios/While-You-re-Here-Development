using UnityEngine;

namespace making_tea
{
    public class KettleFill : MonoBehaviour
    {
        [Header("Fill Settings")]
        public float fillAmount;
        public float maxFill = 1f;
        public float fillSpeed = 0.25f;

        [HideInInspector]
        public bool isFilling;

        private void Update()
        {
            if (!isFilling || !(fillAmount < maxFill)) return;
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp(fillAmount, 0f, maxFill);
        }

        public void StartFilling() => isFilling = true;
        public void StopFilling() => isFilling = false;
    }
}