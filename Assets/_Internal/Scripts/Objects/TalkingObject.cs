using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingObject : MonoBehaviour, IInteractable
{
    public Dialogue dialogue;

    private bool isInDialogue = false;
    public bool OnInteraction(PlayerController player)
    {
        if (isInDialogue)
        {
            DialogueManager.Instance.displayNextSentence();          
        }
        else
        {
            isInDialogue = true;
            player.SetMovement(PlayerMovement.None);
            
            DialogueManager.Instance.StartDialogue(dialogue);
            DialogueManager.Instance.endedDialogue.AddListener(DialogueEnded);
            DialogueManager.Instance.endedDialogue.AddListener(() => { player.SetMovement(PlayerMovement.Continuous); });
        }
        return true;
    }

    private void DialogueEnded()
    {
        isInDialogue = false;
        DialogueManager.Instance.endedDialogue.RemoveListener(DialogueEnded);
    }

    public bool OnInteraction(MonsterController monster)
    {
        throw new System.NotImplementedException();
    }
}
