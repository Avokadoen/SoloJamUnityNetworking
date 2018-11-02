using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

    public void ButtonLevelLoad(GameObject button)
    {
        string sceneName = button.GetComponentInChildren<Text>().text;
        if(sceneName == null)
        {
            Debug.LogError("could not retrieve text from button");
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
