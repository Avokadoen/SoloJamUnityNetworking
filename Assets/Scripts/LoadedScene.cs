using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LoadedScene : MonoBehaviour {

    public NetworkManagerHUD networkHUD;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //networkHUD = gameObject.GetComponent<NetworkManagerHUD>();
        //networkHUD.showGUI = false;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name != "mainMenu")
        {
            networkHUD.showGUI = true;
        }
        else
        {
            networkHUD.showGUI = false;
        }
    }

}
