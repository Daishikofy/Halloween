using Unity;
using UnityEngine;
public class PuzzleItemRewards : Puzzles
{
    [Header("Reward")]
    public CollectibleType item;
    public int quantity;
    public Dialogue dialogue;

    private bool isDialogueRunning;

    public override bool OnInteraction(PlayerController player)
    {
        if (isDialogueRunning)
            DialogueManager.Instance.displayNextSentence();
        return base.OnInteraction(player);
    }

    protected override void OnComplete()
    {
        isDialogueRunning = true;
        GameController.Instance.PlayerEarnsItems(item, quantity);
        DialogueManager.Instance.StartDialogue(dialogue);
        DialogueManager.Instance.endedDialogue.AddListener(DialogueEnds);
    }

    private void DialogueEnds()
    {
        isDialogueRunning = false;
        miniGameStarted = false;
        DialogueManager.Instance.endedDialogue.RemoveListener(DialogueEnds);
        PuzzleEnded();
    }
}

