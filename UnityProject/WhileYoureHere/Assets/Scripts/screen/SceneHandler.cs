using UnityEngine;

namespace screen
{
    public static class SceneHandler
    {
        public static string PreviousScene { get; private set; } = StartScreen;

        public const string StartScreen = "StartScreen";
        public const string OptionsScreen = "OptionsScreen";
        public const string LoadScreen = "LoadScreen";
        public static readonly string[] GameplayScenes = { "Day1", "Day2" };

        public static void SetPreviousScene(string sceneName)
        {
            Debug.Log($"[SceneHandler] PreviousScene set to: {sceneName}");
            PreviousScene = sceneName;
        }

        public static bool IsPreviousSceneGameplay =>
            !string.IsNullOrEmpty(PreviousScene) &&
            System.Array.Exists(GameplayScenes, s => s == PreviousScene);
    }
}