using UnityEngine;

[CreateAssetMenu(fileName = "DialogueChoice", menuName = "Scriptable Objects/DialogueChoice")]
public class DialogueChoice : ScriptableObject
{
    public string choiceText;
    public string targetNodeID;
}
