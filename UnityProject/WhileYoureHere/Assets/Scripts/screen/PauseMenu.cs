using UnityEngine;

namespace screen
{
    public class PauseMenu : MonoBehaviour
    {
        public void OnResumeButton()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
        
        public void OnLoadButton()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScreen");
        }
        
        public void OnSettingsButton()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("OptionsScreen");
        }
        
        public void OnQuitButton()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartScreen");
        }
        
        public void OnQuitDesktopButton()
        {
            Application.Quit();
        }
    }
}