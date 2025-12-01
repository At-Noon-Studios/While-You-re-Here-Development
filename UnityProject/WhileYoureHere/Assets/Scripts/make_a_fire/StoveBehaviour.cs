using UnityEngine;
using UnityEngine.VFX;

namespace make_a_fire
{
    public class StoveBehaviour : MonoBehaviour
    {
        public VisualEffect fireParticle;
        private bool _isFireOn;

        private void Awake()
        {
            PlayFireEffect(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Newspaper") && !_isFireOn)
            {
                Debug.Log("Newspaper collided with " + collision.gameObject.name);
                _isFireOn = true;
                PlayFireEffect(true);
                Destroy(collision.gameObject);
            }
        }

        private void PlayFireEffect(bool status)
        {
            if (fireParticle == null) return;

            fireParticle.gameObject.SetActive(status);
 
            if (status)
            {
                fireParticle.Play();
            }
        }
    }
}