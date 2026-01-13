using UnityEngine;
using UnityEngine.SceneManagement;

namespace screen
{
    public class PlayerSceneHandler : MonoBehaviour
    {
        private void Awake()
        {
            if (!IsGameplayScene(SceneManager.GetActiveScene().name))
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!IsGameplayScene(scene.name))
            {
                Destroy(gameObject);
            }
        }

        private static bool IsGameplayScene(string sceneName)
        {
            foreach (var s in SceneHandler.GameplayScenes)
                if (s == sceneName) return true;

            // Allow Options to keep the object alive
            if (sceneName == SceneHandler.OptionsScreen)
                return true;

            return false;
        }
    }
}