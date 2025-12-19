using System.Collections;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fishing {
    public class FishingRod : HoldableObjectBehaviour
    {
        private PlayerInput _playerInput;
        private bool _isCastPullPressed;
        private bool _isLineCast;
        private bool _isBittenByFish;
        private bool _isCasting;
        private float _castingForce;
        private LineController _lineController;
        private bool _isMaxCharge;

        public float castingForceMultiplier;
        public float chargeCastTiltMultiplier;
        public float maxChargeTimeInMilliseconds;
        
        public GameObject line;
        public GameObject fishingRodTop;
        public GameObject floaterPrefab;

        protected override void Awake()
        {
            base.Awake();
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
            
            _playerInput = player.GetComponent<PlayerInput>();

            if (_playerInput == null) return;
            _playerInput.actions["CastPullFishingRod"].performed += ctx => _isCastPullPressed = true;
            _playerInput.actions["CastPullFishingRod"].canceled += ctx => _isCastPullPressed = false;
            _lineController = GetComponent<LineController>();
        }

        private void Update()
        {
            if (_holder == null) return;
            if (_isCastPullPressed)
            {
                if (!_isLineCast)
                {
                    ChargeCast();
                }
                else PullLine();
            }
            else
            {
                if (_isCasting) CastLine();
            }
        }

        private void ChargeCast()
        {
            if (!_isCasting) StartCoroutine(TimeDownMaxCharge());
            _isCasting = true;
            if (_isMaxCharge) return;
            _castingForce += castingForceMultiplier * Time.deltaTime;
            transform.Rotate(-chargeCastTiltMultiplier, 0, 0);
        }

        private IEnumerator TimeDownMaxCharge()
        {
            _isMaxCharge = false;
            yield return new WaitForSeconds(maxChargeTimeInMilliseconds);
            _isMaxCharge = true;
        }

        private void CastLine()
        {
            var floater = Instantiate(floaterPrefab, fishingRodTop.transform.position, fishingRodTop.transform.rotation);
            floater.gameObject.GetComponent<Rigidbody>().AddForce(_castingForce * _playerCamera.forward, ForceMode.Impulse);
            _lineController.SetUpLine(new []{fishingRodTop.transform, floater.transform});
            line.SetActive(false);
            _isLineCast = true;
            _isCasting = false;
            _castingForce = 0;
        }

        private void PullLine()
        {
            if (_isBittenByFish)
            {
                
            }
            else
            {
                
            }
        }

        private void WobbleRod()
        {
            
        }

        private IEnumerator TriggerFishBite()
        {
            yield return new WaitForSeconds(3);
            _isBittenByFish = true;
            WobbleRod();
        }
    }
}