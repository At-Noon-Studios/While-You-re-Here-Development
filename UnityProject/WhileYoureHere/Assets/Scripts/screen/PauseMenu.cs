using ScriptableObjects.Events;
using PlayerControls;
using UnityEngine;

namespace screen
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private PauseEventChannel pauseEventChannel;

        [Header("References")]
        [SerializeField] private GameObject pauseMenuUI;

        public void OnResumeButton()
        {
            if (pauseEventChannel != null)
                pauseEventChannel.Raise(); // This unpauses
        }
        
        public void OnLoadButton()
        {
            ResetPauseState();
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScreen");
        }
        
        public void OnSettingsButton()
        {
            ResetPauseState();
            UnityEngine.SceneManagement.SceneManager.LoadScene("OptionsScreen");
        }
        
        public void OnQuitButton()
        {
            ResetPauseState();
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartScreen");
        }
        
        public void OnQuitDesktopButton()
        {
            Application.Quit();
        }

        private void ResetPauseState()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Cursor.visible = true;

            if (pauseMenuUI != null)
                pauseMenuUI.SetActive(false);
        }
    }
}