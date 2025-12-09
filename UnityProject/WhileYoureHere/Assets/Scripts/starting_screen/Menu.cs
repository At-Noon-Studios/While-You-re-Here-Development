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
    }
}