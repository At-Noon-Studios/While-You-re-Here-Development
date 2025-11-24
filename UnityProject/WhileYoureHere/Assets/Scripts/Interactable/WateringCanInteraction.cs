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

        // Update is called once per frame
        void Update()
        {

        }

        public override void Interact()
        {
            transform.localEulerAngles = new Vector3(0, 0, 45);
            print("rotate watering can to water");
        }
    }
}
