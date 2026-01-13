using PlayerControls;
using ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace screen
{
    public class PauseMenuController : MonoBehaviour
    {
        [Header("Pause menu settings")]
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private PauseEventChannel pauseEventChannel;

        [Header("Player")]
        [SerializeField] private CameraController cameraController;

        private bool _isPaused;

        private void OnEnable()
        {
            pauseEventChannel.OnRaise += TogglePause;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            pauseEventChannel.OnRaise -= TogglePause;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void TogglePause()
        {
            _isPaused = !_isPaused;

            Time.timeScale = _isPaused ? 0f : 1f;
            AudioListener.pause = _isPaused;

            pauseMenuUI.SetActive(_isPaused);

            if (cameraController != null)
                cameraController.enabled = !_isPaused;

            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _isPaused;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _isPaused = false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            pauseMenuUI.SetActive(false);

            if (cameraController != null)
                cameraController.enabled = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (!IsGameplayScene(scene.name))
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        private static bool IsGameplayScene(string sceneName)
        {
            foreach (var s in SceneNames.GameplayScenes)
                if (s == sceneName) return true;
            return false;
        }
    }
}
