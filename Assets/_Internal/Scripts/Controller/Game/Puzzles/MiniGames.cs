using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class MiniGame : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent onCompleted;
    [HideInInspector]
    public UnityEvent onMissed;

    [Header("Puzzle Tween Animation")]
    public Vector3 openedPosition;
    public Vector3 closedPosition;
    public LeanTweenType verticalTweenType;    
    public LeanTweenType scaleTweenType;
    public float puzzleTweenTime;

    protected bool succes;
    protected bool miniGameStarted;

    public async void ShowMinigame()
    {
        InitializeMiniGame();
        gameObject.LeanMoveLocal(openedPosition, puzzleTweenTime / 2).setEase(verticalTweenType);
        gameObject.LeanScale(Vector3.one, puzzleTweenTime).setEase(scaleTweenType);
        await Task.Delay((int)(puzzleTweenTime * 1000));
        StartMiniGame();
    }

    protected abstract void InitializeMiniGame();

    protected virtual void StartMiniGame()
    {
        miniGameStarted = true;
    }

    public virtual IEnumerator EndMiniGame()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("MiniGame you win: " + succes);
        miniGameStarted = false;
        if (succes)
            onCompleted.Invoke();
        else
            onMissed.Invoke();

        gameObject.LeanScale(Vector3.zero, puzzleTweenTime).setEase(scaleTweenType);
        gameObject.LeanMoveLocal(closedPosition, puzzleTweenTime).setEase(verticalTweenType);
    }
}
