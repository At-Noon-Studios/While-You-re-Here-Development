using UnityEngine;
using UnityEngine.VFX;

namespace make_a_fire
{
    public class StoveBehaviour : MonoBehaviour
    {
        public VisualEffect fireEffect;
        private bool _isFireOn;

        private void Awake()
        {
            PlayFireEffect(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Newspaper") && !_isFireOn)
            {
                _isFireOn = true;
                PlayFireEffect(true);
                Destroy(collision.gameObject);
            }
        }

        private void PlayFireEffect(bool status)
        {
            if (fireEffect == null) return;

            fireEffect.gameObject.SetActive(status);

            if (status)
            {
                fireEffect.Play();
            }
        }
    }
}