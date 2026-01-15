using saving_loading;
using UnityEngine;
using UnityEngine.UI;

namespace screen
{
    public class LoadingMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject loadingConfirmCanvasUI;
        [SerializeField] private GameObject savingConfirmCanvasUI;
        [SerializeField] private GameObject exceedSlotsCanvasUI;
        [SerializeField] private GameObject savingIconUI;
            
        [Header("Button References")]
        [SerializeField] private Button loadingConfirmButton;
        [SerializeField] private Button loadingCancelButton;
        
        private int _slotsFilled = 0;
        private const int TotalSlots = 5;

        private void SaveGame()
        {
            int nextSlot = SaveSystem.GetNextSlot();

            if (nextSlot == -1)
            {
                exceedSlotsCanvasUI.SetActive(true);
                return;
            }

            // Create dummy data or pull from GameStateManager
            SaveData data = new SaveData
                
            {
                lastTaskCompleted = "Task X"
            };

            SaveSystem.Save(data);
            _slotsFilled = SaveSystem.CountSlots();
            UpdateLoadingDisplay();
        }

        private void LoadGame()
        {
            SaveData data = SaveSystem.Load(0); // or selected slot

            if (data != null)
            {
                // Pass data to GameStateManager
                // SceneHandler.LoadScene(data.sceneName);
            }
        }
        
        private void FillSlot()
        {
            if (_slotsFilled < TotalSlots)
            {
                _slotsFilled++;
                UpdateLoadingDisplay();
            }
        }
        
        
        private void UpdateLoadingDisplay()
        {
            Debug.Log($"Loading... {_slotsFilled}/{TotalSlots} slots filled.");
            // Here you can add code to update the UI elements accordingly
        }

        public void OnStartLoading()
        {
            loadingConfirmCanvasUI.SetActive(true);
        }
        
        public void OnConfrimLoading()
        {
            //something here call LoadGame();
        }
        
        public void OnCancelLoading()
        {
            // cancel loading process
        }
        
        public void OnSavingGame()
        {
            if (_slotsFilled >= TotalSlots)
            {
                exceedSlotsCanvasUI.SetActive(true);
            }
            else
            {
                savingConfirmCanvasUI.SetActive(true);
            }
        }
    }
}