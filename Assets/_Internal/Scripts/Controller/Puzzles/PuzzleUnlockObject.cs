using FMODUnity;
using UnityEngine;

public class PuzzleUnlockObject : Puzzles
{
    public HidingObject lockedHidingPlace;
    [FMODUnity.EventRef]
    public string unlockSFX;

    public override bool OnInteraction(PlayerController player)
    {
        Debug.Log("lockedHidingPlace.isLocked: " + lockedHidingPlace.isLocked);
        if (lockedHidingPlace.isLocked)
            return base.OnInteraction(player);
        return lockedHidingPlace.OnInteraction(player);
    }

    protected override void OnComplete()
    {
        RuntimeManager.PlayOneShot(unlockSFX);
        lockedHidingPlace.isLocked = false;
        miniGameStarted = false;
        PuzzleEnded();
    }
}
