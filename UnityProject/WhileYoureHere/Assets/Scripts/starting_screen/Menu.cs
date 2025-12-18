using System;
using System.Collections;
using UnityEngine;

namespace starting_screen
{
    public class Menu : MonoBehaviour
    {
        public void OnPlayButton()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Day1");
        }
        
        public void OnQuitButton()
        {
            Application.Quit();
        }
        
        public void BackToMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartScreen");
        }

        public void OnSettingsButton()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("OptionsScreen");
        }
        
        public void OnLoadButton()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScreen");
        }
        
    }
}