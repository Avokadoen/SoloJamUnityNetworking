using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainMenuScript : MonoBehaviour {

    public NetworkManager networkManager;

    public void ButtonLevelLoad(GameObject button)
    {
        string sceneName = button.GetComponentInChildren<Text>().text;
        if(sceneName == null)
        {
            Debug.LogError("could not retrieve text from button");
            return;
        }

        networkManager.onlineScene = sceneName;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
