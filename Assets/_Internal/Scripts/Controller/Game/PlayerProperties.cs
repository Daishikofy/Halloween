using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
    private static PlayerProperties instance;
    public static PlayerProperties Instance { get { return instance; } }
    private PlayerData playerData = new PlayerData();
    public bool wasChanged = true;

    public InventoryController inventory 
    { 
        get 
        { 
            return playerData.inventory; 
        } 
        set
        {
            playerData.inventory = value;
            wasChanged = true;
        }
    }
    public string scene
    {
        get
        {
            return playerData.currentScene;
        }
        set
        {
            playerData.currentScene = value;
            wasChanged = true;
        }
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void ClearData()
    {
        System.IO.File.Delete(Application.persistentDataPath + "/Data.json");
    }

    public void Save()
    {
        string data = JsonUtility.ToJson(playerData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/Data.json", data);
        wasChanged = false;
    }

    public bool Load()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/Data.json"))
            return false;
        wasChanged = false;
        var data = System.IO.File.ReadAllText(Application.persistentDataPath + "/Data.json");
        playerData = JsonUtility.FromJson<PlayerData>(data);
        return true;
    }
}

[Serializable]
public class PlayerData
{
    public InventoryController inventory;
    public string currentScene;
}
