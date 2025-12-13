using System.Collections.Generic;
using door;
using gamestate;
using UnityEngine;

public class GlobalTriggerManager : MonoBehaviour
{
    [SerializeField] private List<DoorInteractable> doorsToLock;
    private bool startListeningForEndDay = false;

    public bool StartListeningForEndDay
    {
        set => startListeningForEndDay = value;
    }

    private void Update()
    {
        if (startListeningForEndDay) ListenForEndDay();
    }
    
    private void ListenForEndDay()
    {
        foreach (var door in doorsToLock)
        {
            if (!door.isLocked) return;
        }
        GamestateManager.GetInstance().listOfFlags.Find(flag => flag.name == "DoorsLocked").currentValue = true;
    }
}
