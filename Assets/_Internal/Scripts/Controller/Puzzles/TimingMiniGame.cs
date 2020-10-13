using System.Collections;
using UnityEngine;

public class TimingMiniGame : MiniGame
{
    [Header("MiniGame Object Setup")]
    public GameObject movingPart;
    public Vector2 correctRegion;
    public float leftPosition;
    public float rightPosition;
    public float trajectoryDuration;
    public LeanTweenType knobTweenType;

    protected override void InitializeMiniGame()
    {
        var aux = movingPart.transform.localPosition;
        aux.x = leftPosition;
        movingPart.transform.localPosition = aux; 
    }

    protected override void StartMiniGame()
    {
        movingPart.LeanMoveLocalX(rightPosition, trajectoryDuration).setEase(knobTweenType).setLoopPingPong();
        base.StartMiniGame();
    }
    private void Update()
    {
        if (!miniGameStarted) return;
        if(Input.GetKeyDown(KeyCode.E))
        {
            var value = movingPart.transform.localPosition.x;
            if (value < correctRegion.y && value > correctRegion.x)
                succes = true;
            else
                succes = false;
            movingPart.LeanCancel();
            StartCoroutine(EndMiniGame());
        }
    }
}
