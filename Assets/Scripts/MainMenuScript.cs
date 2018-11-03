using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainMenuScript : MonoBehaviour {

    public GameObject[] Characters;

    public void ButtonLevelLoad(GameObject button)
    {
        string sceneName = button.GetComponentInChildren<Text>().text;
        if(sceneName == null)
        {
            Debug.LogError("could not retrieve text from button");
            return;
        }

        NetworkManager.singleton.onlineScene = sceneName;
        NetworkManager.singleton.ServerChangeScene(sceneName);

    }

    public void ButtonCharacterLoad(GameObject button)
    {
        string sceneName = button.GetComponentInChildren<Text>().text;
        if (sceneName == null)
        {
            Debug.LogError("could not retrieve text from button");
            return;
        }

        SceneManager.LoadScene(sceneName);

    }

    public void SelectCharacter(GameObject button)
    {
        int index = 0;
        if (button.name == "UglyMan")
        {
            index = 0;
        }
        else if(button.name == "HotDad"){
            index = 1;
        }

        NetworkManager.singleton.playerPrefab = Characters[index];
    }
}
