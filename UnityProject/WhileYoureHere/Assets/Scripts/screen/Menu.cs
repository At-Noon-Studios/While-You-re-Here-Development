using making_tea;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace screen
{
    public class Menu : MonoBehaviour
    {
        public void OnPlayButton()
        {
            ResetPauseState();
            ChairInteractable.ResetChairState();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneHandler.SetPreviousScene("Day1");
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

        public void OnSettingsButton()
        {
            var current = SceneManager.GetActiveScene().name;

            if (System.Array.Exists(SceneHandler.GameplayScenes, s => s == current))
            {
                SceneHandler.SetPreviousScene(current);
            }

            SceneManager.LoadScene(SceneHandler.OptionsScreen);
        }
        
        public void BackToMenu()
        {
            Debug.Log($"[Menu] BackToMenu → PreviousScene = {SceneHandler.PreviousScene}");
            ResetPauseState();
            SceneManager.LoadScene(SceneHandler.PreviousScene);
        }

        public void OnLoadButton()
        {
            SceneHandler.SetPreviousScene(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(SceneHandler.LoadScreen);
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }

        private void ResetPauseState()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Cursor.visible = true;
        }
    }
}