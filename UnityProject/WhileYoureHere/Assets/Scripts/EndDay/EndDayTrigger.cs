using System.Collections.Generic;
using door;
using gamestate;
using UnityEngine;

namespace EndDay
{
    public class EndDayTrigger : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float displayImageDuration = 1f;
        [SerializeField] private GameObject player;
        [SerializeField] private CanvasGroup endDayCanvasGroup;
        [SerializeField] private List<DoorInteractable> doorsToLock;

        private bool startListeningForEndDay = false;
        
        private float _timer;
        
        public bool StartListeningForEndDay
        {
            set => startListeningForEndDay = value;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player)
            {
                EndDay();
            }
        }

        private void Update()
        {
            if (!startListeningForEndDay) return;
            foreach (var door in doorsToLock)
            {
                if (!door.isLocked) return;
            }

            GamestateManager.GetInstance().listOfFlags.Find(flag => flag.name == "DoorsLocked").currentValue = true;
        }

        private void EndDay()
        {
            endDayCanvasGroup.alpha = _timer / fadeDuration;
            _timer += Time.deltaTime;

            if (_timer > fadeDuration + displayImageDuration)
            {
                // quit game (or maybe change scene) can be added here 
            }
        }
    }
}
