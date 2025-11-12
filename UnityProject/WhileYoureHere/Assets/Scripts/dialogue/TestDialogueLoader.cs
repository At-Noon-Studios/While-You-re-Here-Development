using System.Collections.Generic;
using UnityEngine;

public class TestDialogueLoader : MonoBehaviour
{
    void Start()
    {
        List<DialogueNode> nodes = new List<DialogueNode>();
        
        nodes.Add(new DialogueNode
        {
            nodeID = "start",
            speakerName = "Stranger:",
            dialogueText = "Hey! Did you just lock the fucking door?! Who do you think you are?",
            choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceText = "I am so so sorry, I'm Nora...", targetNodeID = "response1" },
                new DialogueChoice { choiceText = "Who am I?? Who are you!", targetNodeID = "response2" },
                new DialogueChoice { choiceText = "Uhh... Cabina... Dawoods..?", targetNodeID = "response3" }
            }
        });

        // RESPONSE 1
        nodes.Add(new DialogueNode
        {
            nodeID = "response1",
            speakerName = "Stranger:",
            dialogueText = "Nora...? You better explain yourself quickly! Do you even know what you just did?",
            choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceText = "I panicked, okay? Thought someone was outside.", targetNodeID = "response1a" },
                new DialogueChoice { choiceText = "No… I didn’t know. I'm sorry.", targetNodeID = "response1b" }
            }
        });

        nodes.Add(new DialogueNode
        {
            nodeID = "response1a",
            speakerName = "Stranger:",
            dialogueText = "Panicked?? That doesn’t explain locking ME inside!",
            choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceText = "I’ll unlock it, just calm down.", targetNodeID = "end_good" },
                new DialogueChoice { choiceText = "Maybe you deserved it.", targetNodeID = "end_bad" }
            }
        });

        nodes.Add(new DialogueNode
        {
            nodeID = "response1b",
            speakerName = "Stranger:",
            dialogueText = "...At least you're honest. But seriously, open the door. Now.",
            choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceText = "Fine, fine. I’ll open it.", targetNodeID = "end_good" },
                new DialogueChoice { choiceText = "Not until you tell me who YOU are.", targetNodeID = "response2" }
            }
        });

        // RESPONSE 2
        nodes.Add(new DialogueNode
        {
            nodeID = "response2",
            speakerName = "Stranger:",
            dialogueText = "DO NOT ANSWER A QUESTION WITH ANOTHER QUESTION!",
            choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceText = "Okay! I'm sorry, jeez...", targetNodeID = "response2a" },
                new DialogueChoice { choiceText = "You sound like a teacher.", targetNodeID = "response2b" }
            }
        });

        nodes.Add(new DialogueNode
        {
            nodeID = "response2a",
            speakerName = "Stranger:",
            dialogueText = "...Just open the door. I don’t want trouble.",
            choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceText = "Alright, I’ll open it.", targetNodeID = "end_good" },
                new DialogueChoice { choiceText = "Not until you calm down.", targetNodeID = "end_mixed" }
            }
        });

        nodes.Add(new DialogueNode
        {
            nodeID = "response2b",
            speakerName = "Stranger:",
            dialogueText = "A TEACHER?! I swear if you don’t unlock this door—",
            choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceText = "Okay, okay! I’ll unlock it.", targetNodeID = "end_good" },
                new DialogueChoice { choiceText = "Try to break it then.", targetNodeID = "end_bad" }
            }
        });

        // RESPONSE 3
        nodes.Add(new DialogueNode
        {
            nodeID = "response3",
            speakerName = "Stranger:",
            dialogueText = "... That's clearly a fake name... LET ME OUT!",
            choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceText = "Okay! Fine! Don’t yell!", targetNodeID = "end_good" },
                new DialogueChoice { choiceText = "It’s not fake! I swear!", targetNodeID = "response3a" }
            }
        });

        nodes.Add(new DialogueNode
        {
            nodeID = "response3a",
            speakerName = "Stranger:",
            dialogueText = "Whatever. Open the damn door. Now.",
            choices = new List<DialogueChoice>
            {
                new DialogueChoice { choiceText = "Okay… I’ll open it.", targetNodeID = "end_good" },
                new DialogueChoice { choiceText = "Not until you say please.", targetNodeID = "end_bad" }
            }
        });

        // ENDINGS
        nodes.Add(new DialogueNode
        {
            nodeID = "end_good",
            speakerName = "Stranger:",
            dialogueText = "Finally. Thank you. That wasn't so hard, was it?",
            choices = new List<DialogueChoice>()
        });

        nodes.Add(new DialogueNode
        {
            nodeID = "end_bad",
            speakerName = "Stranger:",
            dialogueText = "You’re unbelievable. I’m getting out of here myself.",
            choices = new List<DialogueChoice>()
        });

        nodes.Add(new DialogueNode
        {
            nodeID = "end_mixed",
            speakerName = "Stranger:",
            dialogueText = "...Fine. Just hurry up before someone sees us.",
            choices = new List<DialogueChoice>()
        });

        DialogueManager dm = FindObjectOfType<DialogueManager>();
        if (dm != null)
        {
            dm.StartDialogue(nodes, "start");
        }
        else
        {
            Debug.LogError("DialogueManager not found in the scene!");
        }
    }
}
