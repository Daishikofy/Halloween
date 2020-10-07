using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour, IInteractable
{
    public bool OnInteraction(PlayerController player)
    {
        GameController.Instance.OnPlayerWin();
        return true;
    }

    public bool OnInteraction(MonsterController monster)
    {
        return false;
    }
}
