using System.Collections;
using UnityEngine;

namespace Interactable
{
    public class WateringCanInteraction : InteractableBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        public override void Interact()
        {
            
            transform.localEulerAngles = new Vector3(0, 0, 45);
            print("rotate watering can to water");
            
            StartCoroutine(StartWatering());
        }

        private IEnumerator StartWatering()
        {
            enabled = false;
            
            yield return new WaitForSeconds(2.5f);
            transform.localEulerAngles = new Vector3(0, 0, 0);
            print("rotate watering can back");
            
            enabled = true;
        }
    }
}
