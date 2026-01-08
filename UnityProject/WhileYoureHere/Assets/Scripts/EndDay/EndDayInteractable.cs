using System.Collections;
using Interactable;
using player_controls;
using PlayerControls;
using UnityEngine;
using UnityEngine.Video;

namespace EndDay
{
    public class EndDayInteractable : InteractableBehaviour
    {
        private float _timer;
        private CameraController _cameraController;
        private MovementController _movementController;
        private VideoPlayer _videoPlayer;
        private AudioSource _audioSource;

        [SerializeField] private AudioClip endOfDaySound;

        protected override void Awake()
        {
            base.Awake();
            var player = GameObject.FindGameObjectWithTag("Player");
            _cameraController = player.GetComponentInChildren<CameraController>();
            _movementController = player.GetComponentInChildren<MovementController>();
            _videoPlayer = GetComponent<VideoPlayer>();
            _audioSource = GetComponent<AudioSource>();
        }

        public override void Interact(IInteractor interactor)
        {
            _cameraController.PauseCameraMovement();
            _movementController.PauseMovement();
            blockInteraction = true;
            // _videoPlayer.Play(); Not functional yet, commented to prevent error
            _audioSource.PlayOneShot(endOfDaySound);
            // StartCoroutine(GoToNextScene()); Not functional yet, commented to prevent error
        }

        private IEnumerator GoToNextScene()
        {
            //prepare new scene?
            yield return new WaitForSeconds((float)_videoPlayer.clip.length);
            //go to new scene
        }

        public override string InteractionText(IInteractor interactor)
        {
            return "End the day";
        }
    }
}