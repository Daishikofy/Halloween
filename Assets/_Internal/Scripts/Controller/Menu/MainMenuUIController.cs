using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header("Global Setup")]
    public MainMenuController controller;
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;
    public GameObject creditsPanel;

    [Header("Main Menu Panel")]
    public Button continueButton;

    [Header("Global Setup")]

    private GameObject currentPanel;
    // Start is called before the first frame update
    void Start()
    {
        mainMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        currentPanel = mainMenuPanel;

        continueButton.interactable = PlayerProperties.Instance.Load();
    }

    public void OnClearDataClicked()
    {
        PlayerProperties.Instance.ClearData();
    }

    public void OnCreditsClick()
    {
        ActivatePanel(creditsPanel);
    }

    public void OnControlsClick()
    {
        ActivatePanel(controlsPanel);
    }

    public void OnBackClick()
    {
        ActivatePanel(mainMenuPanel);
    }

    public void OnLoadGameClick()
    {
        controller.LoadGame();
    }

    public void OnNewGameClick()
    {
        controller.NewGame();
    }

    private void ActivatePanel(GameObject panel)
    {
        currentPanel.SetActive(false);
        panel.SetActive(true);
        currentPanel = panel;
    }
}
