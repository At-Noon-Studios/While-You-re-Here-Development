using UnityEngine;
using UnityEngine.SceneManagement;

namespace screen
{
    public class PlayerSceneHandler : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            if (IsGameplayScene(SceneManager.GetActiveScene().name))
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
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
            foreach (var s in SceneNames.GameplayScenes)
            {
                if (s == sceneName) return true;
            }

            return false;
        }
    }
}