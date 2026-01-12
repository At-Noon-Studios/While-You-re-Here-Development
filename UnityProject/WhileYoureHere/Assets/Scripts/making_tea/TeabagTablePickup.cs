using Interactable;
using UnityEngine;

namespace making_tea
{
    public class TeabagTablePickup : TablePickup
    {
        private Collider _col;

        protected override void Awake()
        {
            enableRailDrag = true;
            base.Awake();
            _col = GetComponent<Collider>();
        }

        private void OnDestroy()
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;

            var pic = player.GetComponent<PlayerInteractionController>();
            if (pic == null) return;

            pic.UnregisterTablePickup(this);
        }

        public override void EnableCollider(bool s)
        {
            if (_col) _col.enabled = s;
        }
    }
}