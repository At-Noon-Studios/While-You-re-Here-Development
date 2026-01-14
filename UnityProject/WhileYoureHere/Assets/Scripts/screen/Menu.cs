using making_tea;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace screen
{
    public class Menu : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject optionsMenuUI;
        [SerializeField] private GameObject startMenuUI;
        
        public void OnPlayButton()
        {
            ResetPauseState();
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

        public void OnSettingsButton()
        {
            optionsMenuUI.SetActive(true);
            startMenuUI.SetActive(false);
        }

        public void OnSettingsBack()
        {
            optionsMenuUI.SetActive(false);
            startMenuUI.SetActive(true);
        }

        public void OnLoadButton()
        {
            ResetPauseState();
            SceneManager.LoadScene("LoadScreen");
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }

        private static void ResetPauseState()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Cursor.visible = true;
        }
    }
}