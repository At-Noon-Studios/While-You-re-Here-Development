using UnityEngine;

namespace EndDay
{
    public class EndDayTrigger : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float displayImageDuration = 1f;
        [SerializeField] private GameObject player;
        [SerializeField] private CanvasGroup endDayCanvasGroup;

        private float _timer;
        private bool _isEndDay = false;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player)
            {
                _isEndDay = true;
            }
        }

        void Update()
        {
            if (_isEndDay)
            {
                EndDay();
            }
        }

        private void EndDay()
        {
            endDayCanvasGroup.alpha = _timer / fadeDuration;
            _timer += Time.deltaTime;

            if (_timer > fadeDuration + displayImageDuration)
            {
                Debug.Log("End of the day");
            }
        }
    }
}
