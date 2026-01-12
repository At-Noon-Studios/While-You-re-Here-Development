using System;
using Interactable;
using PlayerControls;
using UnityEngine;
using UnityEngine.Playables;

namespace Playable
{
    public class PlayableManager : MonoBehaviour
    {
        public static PlayableManager Instance;
    
        private CameraController _cameraController;
        private MovementController _movementController;
        private PlayerInteractionController _interactionController;
    
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        
            var player = GameObject.FindWithTag("Player");
            _cameraController = player.GetComponentInChildren<CameraController>();
            _movementController = player.GetComponent<MovementController>();
            _interactionController = player.GetComponent<PlayerInteractionController>();
        }
    
        public void Play(PlayableDirector director, Action onComplete = null)
        {
            director.Play();
            _cameraController.PauseCameraMovement();
            _movementController.PauseMovement();
            _interactionController.PausePlayerInteraction();
            director.stopped += (dir) =>
            {
                Resume(dir);
                onComplete?.Invoke();
            };
        }

        private void Resume(PlayableDirector director)
        {
            director.stopped -= Resume;
            _cameraController.ResumeCameraMovement();
            _movementController.ResumeMovement();
            _interactionController.ResumePlayerInteraction();
        }
    }
}
