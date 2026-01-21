using ScriptableObjects.Events;
using UnityEngine;

namespace screen
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private PauseEventChannel pauseEventChannel;

        [Header("References")] 
        [SerializeField] private GameObject pauseMenuUI;

        [SerializeField] private GameObject optionsMenuUI;

        public void OnResumeButton()
        {
            if (pauseEventChannel != null)
                pauseEventChannel.Raise();
        }

        public void OnLoadButton()
        {
            ResetPauseState();
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScreen");
        }

        public void OnOptionsButton()
        {
            pauseMenuUI.SetActive(false);
            optionsMenuUI.SetActive(true);
        }

        public void OnOptionsBack()
        {
            optionsMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
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