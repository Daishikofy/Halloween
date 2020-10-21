using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private string firstGameScene = "";
    void Start()
    {
        
    }

    public void LoadGame()
    {
        PlayerProperties.Instance.Load();
        SceneManager.LoadScene(PlayerProperties.Instance.scene);
    }

    public void NewGame()
    {
        PlayerProperties.Instance.ClearData();
        SceneManager.LoadScene(firstGameScene);
    }
}
