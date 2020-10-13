using FMODUnity;
using UnityEngine;

public class PuzzleUnlockObject : Puzzles
{
    public HidingObject lockedHidingPlace;
    [FMODUnity.EventRef]
    public string unlockSFX;
    private PlayerController player;


    public override bool OnInteraction(PlayerController player)
    {
        this.player = player;
        Debug.Log("lockedHidingPlace.isLocked: " + lockedHidingPlace.isLocked);
        if (lockedHidingPlace.isLocked)
            return base.OnInteraction(player);
        return lockedHidingPlace.OnInteraction(player);
    }

    protected override void OnComplete()
    {
        RuntimeManager.PlayOneShot(unlockSFX);
        lockedHidingPlace.isLocked = false;
        OnInteraction(player);
        miniGameStarted = false;
        PuzzleEnded();
        player = null;
        lockedHidingPlace.isLocked = true;
    }
}
