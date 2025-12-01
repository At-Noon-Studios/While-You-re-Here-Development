using chore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace making_tea
{
    public class KettlePour : MonoBehaviour
    {
        public KettleFill kettle;
        public KettleFill targetCup;
        public ParticleSystem pourStream;
        public AudioSource pourSound;
        public AudioSource spillSound;
        public Transform pivot;

        public float pourAngle = 120f;
        public float rotateSpeed = 8f;
        public float pourSpeed = 0.25f;

        private Rigidbody _rb;
        private Quaternion _uprightRot;
        private Quaternion _pourRot;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();

            if (pivot == null)
                pivot = transform;

            _uprightRot = pivot.localRotation;
            _pourRot = Quaternion.Euler(
                pivot.localEulerAngles.x,
                pivot.localEulerAngles.y,
                pivot.localEulerAngles.z - pourAngle
            );
        }

        private void Update()
        {
            var isHeld = !_rb.useGravity;
            var leftClick = Mouse.current != null && Mouse.current.leftButton.isPressed;
            
            var wantsPour =
                isHeld &&
                leftClick &&
                kettle.fillAmount > 0f;

            if (wantsPour)
            {
                pivot.localRotation = Quaternion.Lerp(
                    pivot.localRotation,
                    _pourRot,
                    Time.deltaTime * rotateSpeed
                );

                if (pourStream && !pourStream.isPlaying)
                    pourStream.Play();
                
                if (pourSound && !pourSound.isPlaying)
                    pourSound.Play();

                var delta = pourSpeed * Time.deltaTime;
                var give = Mathf.Min(delta, kettle.fillAmount);

                kettle.fillAmount -= give;
                if (targetCup)
                    targetCup.fillAmount += give;
                
                if (targetCup && targetCup.fillAmount >= 0.2f)
                {
                    ChoreEvents.TriggerCupFilled();
                }
                if (targetCup && targetCup.fillAmount > targetCup.maxFill)
                {
                    if (!spillSound.isPlaying)
                        spillSound.Play();
                }
                else
                {
                    if (spillSound.isPlaying)
                        spillSound.Stop();
                }
            }
            else
            {
                pivot.localRotation = Quaternion.Lerp(
                    pivot.localRotation,
                    _uprightRot,
                    Time.deltaTime * rotateSpeed
                );

                if (pourStream && pourStream.isPlaying)
                    pourStream.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                
                if (pourSound && pourSound.isPlaying)
                    pourSound.Stop();
                
                if (spillSound && spillSound.isPlaying)
                    spillSound.Stop();
            }
        }
    }
}
