using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindLocalPlayer : MonoBehaviour {
    
	// Update is called once per frame
	void Update () {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(var player in players)
        {
            
        }
    }
}
