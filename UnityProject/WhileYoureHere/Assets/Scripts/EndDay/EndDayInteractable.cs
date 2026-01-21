using System.Collections;
using chopping_logs;
using Interactable;
using Interactable.Holdable;
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
            _videoPlayer.Play();
            _audioSource.PlayOneShot(endOfDaySound);
            StartCoroutine(GoToNextScene());
        }

        private IEnumerator GoToNextScene()
        {
            yield return new WaitForSeconds((float)_videoPlayer.clip.length);
            _videoPlayer.Stop();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            
            var holdable = FindObjectsOfType<HoldableObjectBehaviour>();
            foreach (var h in holdable)
            {
                Destroy(h.gameObject);
            }
            
            var holdableLog = FindObjectsOfType<HalfLogInteractable>();
            foreach (var h in holdableLog)
            {
                Destroy(h.gameObject);
            }
        }

        public override string InteractionText(IInteractor interactor)
        {
            return "End the day";
        }
    }
}