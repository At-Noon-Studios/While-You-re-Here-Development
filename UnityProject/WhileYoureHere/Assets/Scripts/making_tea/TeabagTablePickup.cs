using UnityEngine;

namespace making_tea
{
    public class TeabagTablePickup : TablePickup
    {
        private Collider _col;

        protected override void Awake()
        {
            base.Awake();
            _col = GetComponent<Collider>();
        }

        public override void EnableCollider(bool s)
        {
            if (_col) _col.enabled = s;
        }
    }
}