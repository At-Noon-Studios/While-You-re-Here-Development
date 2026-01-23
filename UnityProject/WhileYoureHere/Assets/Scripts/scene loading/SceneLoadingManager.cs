using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace scene_loading
{
    public class SceneLoadingManager : MonoBehaviour
    {
        public static SceneLoadingManager Instance;
        public static bool Finished;
        public static Action OnFinish;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
        }
    
        private void Start()
        {
            // SceneManager.LoadScene("Day1", LoadSceneMode.Additive);
            SceneManager.LoadScene("Day1Player", LoadSceneMode.Additive);
            SceneManager.LoadScene("Day1Services", LoadSceneMode.Additive);
            SceneManager.LoadScene("Day1Geometry", LoadSceneMode.Additive);
            Finished = true;
            OnFinish?.Invoke();
        }
        
    }
}
