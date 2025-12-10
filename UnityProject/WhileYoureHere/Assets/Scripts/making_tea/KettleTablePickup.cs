using UnityEngine;
using UnityEngine.InputSystem;

namespace making_tea
{
    public class KettleTablePickup : TablePickup
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

        protected override void Update()
        {
            base.Update();

            if (!Lifted || Pic == null || !Pic.TableMode)
                return;

            if (!Mouse.current.leftButton.isPressed) return;
            
            var pour = GetComponent<KettlePour>();
            if (pour != null)
                pour.enabled = true;
        }
    }
}