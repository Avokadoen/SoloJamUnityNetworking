using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FindLocalPlayer : MonoBehaviour {
    
	// Update is called once per frame
	void Update () {
        NetworkBehaviour[] players = NetworkBehaviour.("Player");
        foreach(var player in players)
        {
            
        }
    }
}
