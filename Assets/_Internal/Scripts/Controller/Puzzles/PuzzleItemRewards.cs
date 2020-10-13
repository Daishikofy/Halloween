using Unity;
using UnityEngine;
public class PuzzleItemRewards : Puzzles
{
    [Header("Reward")]
    public CollectibleType item;
    public int quantity;
    public Dialogue itemFound;
    public Dialogue nothingInHere;

    private bool isDialogueRunning;

    public override bool OnInteraction(PlayerController player)
    {
        Debug.Log("OnInteraction");
        if (isDialogueRunning)
        {
            DialogueManager.Instance.displayNextSentence();
            Debug.Log("Next sentence");
        }
        return base.OnInteraction(player);
    }

    protected override void OnComplete()
    {
        isDialogueRunning = true;
        if (quantity > 0)
        {
            GameController.Instance.PlayerEarnsItems(item, 1);
            DialogueManager.Instance.StartDialogue(itemFound);
            quantity--;
        }
        else
        {
            DialogueManager.Instance.StartDialogue(nothingInHere);
        }
            DialogueManager.Instance.endedDialogue.AddListener(DialogueEnds);
    }

    private void DialogueEnds()
    {
        Debug.Log("Dialog ends");
        isDialogueRunning = false;
        miniGameStarted = false;
        DialogueManager.Instance.endedDialogue.RemoveListener(DialogueEnds);
        PuzzleEnded();
    }
}

