using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Scene]
    public string firstGameScene;

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
