using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameUIController : MonoBehaviour
{
    public GameObject victoryScreen;
    public GameObject defeatScreen;

    [HideInInspector]
    public UnityEvent onRestart = new UnityEvent();

    private void Start()
    {
        victoryScreen.SetActive(false);
        defeatScreen.SetActive(false);
    }

    public void OnGameEnded(bool value)
    {
        victoryScreen.SetActive(value);
        defeatScreen.SetActive(!value);
    }

    public void OnRestart()
    {
        onRestart.Invoke();
    }
}
