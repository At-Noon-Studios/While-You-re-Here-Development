using ScriptableObjects.chores;
using UnityEngine;

[CreateAssetMenu(fileName = "SoCcPaperPlacement", menuName = "Scriptable Objects/SoCcPaperPlacement")]
public class SoCcPaperPlacement : SoChoreComponent
{
    [Header("Paper Placement Configuration")]
    public int paperID;
    public int paperAmountNeeded;
    
}
