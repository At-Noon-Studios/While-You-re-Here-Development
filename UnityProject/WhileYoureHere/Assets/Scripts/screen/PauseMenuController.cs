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

        private bool _isPaused;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (pauseEventChannel != null)
                pauseEventChannel.OnRaise += TogglePause;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            if (pauseEventChannel != null)
                pauseEventChannel.OnRaise -= TogglePause;

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void TogglePause()
        {
            _isPaused = !_isPaused;

            Time.timeScale = _isPaused ? 0f : 1f;
            AudioListener.pause = _isPaused;

            if (pauseMenuUI != null)
                pauseMenuUI.SetActive(_isPaused);

            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _isPaused;

            // Only block camera input if a CameraController exists in the current scene
            var cam = FindObjectOfType<CameraController>();
            if (cam != null)
                cam.enabled = !_isPaused;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // If scene is not gameplay, hide pause menu and reset state
            if (!IsGameplayScene(scene.name))
            {
                _isPaused = false;
                Time.timeScale = 1f;
                AudioListener.pause = false;
                if (pauseMenuUI != null)
                    pauseMenuUI.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }

            // Gameplay scene: re-apply pause state
            var cam = FindObjectOfType<CameraController>();
            if (cam != null)
                cam.enabled = !_isPaused;

            if (pauseMenuUI != null)
                pauseMenuUI.SetActive(_isPaused);

            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _isPaused;
        }

        private static bool IsGameplayScene(string sceneName)
        {
            foreach (var s in SceneHandler.GameplayScenes)
                if (s == sceneName) return true;
            return false;
        }
    }
}
