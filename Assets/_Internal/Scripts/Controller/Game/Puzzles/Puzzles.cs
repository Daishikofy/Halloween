using FMODUnity;
using UnityEngine;

public abstract class Puzzles : MonoBehaviour, IInteractable
{
    public MiniGame miniGame;
    [EventRef]
    public string onMissedSFX;

    protected bool miniGameStarted = false;

    public virtual bool OnInteraction(PlayerController player)
    {
        if (!miniGameStarted)
        {
            GameController.Instance.BlockPlayer(true);
            miniGame.onCompleted.AddListener(OnComplete);
            miniGame.onMissed.AddListener(OnMissed);
            miniGameStarted = true;
            miniGame.ShowMinigame();
            return true;
        }
        return true;
    }

    protected abstract void OnComplete();

    private void OnMissed()
    { 
        RuntimeManager.PlayOneShot(onMissedSFX);
        GameController.Instance.PlayerMissedPuzzle();
        miniGameStarted = false;
        Debug.Log("Sound emmited");
        PuzzleEnded();
    }

    protected void PuzzleEnded()
    {
        GameController.Instance.BlockPlayer(false);
        miniGame.onCompleted.RemoveListener(OnComplete);
        miniGame.onMissed.RemoveListener(OnMissed);
    }

    public bool OnInteraction(MonsterController monster)
    {
        throw new System.NotImplementedException();
    }
}
