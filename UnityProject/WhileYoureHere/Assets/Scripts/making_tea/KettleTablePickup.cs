using UnityEngine;
using UnityEngine.InputSystem;

namespace making_tea
{
    public class KettleTablePickup : TablePickup
    {
        private Collider _col;
        private PlayerInput _playerInput;

        protected override void Awake()
        {
            base.Awake();
            _col = GetComponent<Collider>();

            var player = GameObject.FindWithTag("Player");
            if (player != null)
                _playerInput = player.GetComponent<PlayerInput>();
        }

        public override void EnableCollider(bool s)
        {
            if (_col) _col.enabled = s;
        }

        protected override void Update()
        {
            base.Update();

            if (!IsLifted || Pic == null || !Pic.IsTableMode)
                return;

            if (_playerInput == null) return;

            var isPour = _playerInput.actions["Pour"].IsPressed();

            if (!isPour) return;

            var pour = GetComponent<KettlePour>();
            if (pour != null)
                pour.enabled = true;
        }
    }
}