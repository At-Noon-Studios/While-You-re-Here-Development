using ScriptableObjects.Events;
using UnityEngine;

namespace screen
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private EventChannel pauseEvent;
        [SerializeField] private GameObject pauseMenu;

        private void OnEnable()
        {
            pauseEvent.OnRaise += TogglePauseMenu;
        }

        private void OnDisable()
        {
            pauseEvent.OnRaise -= TogglePauseMenu;
        }

        private void TogglePauseMenu()
        {
            var isActive = !pauseMenu.activeSelf;
            pauseMenu.SetActive(isActive);
            Time.timeScale = isActive ? 0f : 1f;
        }
    }
}