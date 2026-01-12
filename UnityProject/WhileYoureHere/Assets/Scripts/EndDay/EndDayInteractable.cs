using System.Collections;
using Interactable;
using player_controls;
using PlayerControls;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace EndDay
{
    public class EndDayInteractable : InteractableBehaviour
    {
        private float _timer;
        private CameraController _cameraController;
        private MovementController _movementController;
        private VideoPlayer _videoPlayer;

        protected override void Awake()
        {
            base.Awake();
            var player = GameObject.FindGameObjectWithTag("Player");
            _cameraController = player.GetComponentInChildren<CameraController>();
            _movementController = player.GetComponentInChildren<MovementController>();
            _videoPlayer = GetComponent<VideoPlayer>();
        }
        
        public override void Interact(IInteractor interactor)
        {
            _cameraController.PauseCameraMovement();
            _movementController.PauseMovement();
            blockInteraction = true;
            _videoPlayer.Play();
            StartCoroutine(GoToNextScene());
        }

        private IEnumerator GoToNextScene()
        {
            yield return new WaitForSeconds((float)_videoPlayer.clip.length);
            _videoPlayer.Stop();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        }
        
        public override string InteractionText(IInteractor interactor)
        {
            return "End the day";
        }
    }
}