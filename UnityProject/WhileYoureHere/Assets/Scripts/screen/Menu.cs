using making_tea;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace screen
{
    public class Menu : MonoBehaviour
    {

        public void OnPlayButton()
        {
            ChairInteractable.ResetChairState();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("Day1");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Day1") return;

            var startingChair = FindObjectOfType<ChairInteractable>();
            if (startingChair != null)
                startingChair.SitAtStart();

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("StartScreen");
        }

        public void OnSettingsButton()
        {
            SceneManager.LoadScene("OptionsScreen");
        }

        public void OnLoadButton()
        {
            SceneManager.LoadScene("LoadScreen");
        }
    }
}