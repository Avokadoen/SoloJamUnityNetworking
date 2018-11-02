using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerInterface))]
public class InputScript : NetworkBehaviour
{

    PlayerInterface playerInterface;

    private void Start()
    {
        playerInterface = GetComponent<PlayerInterface>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            // exit from update if this is not the local player
            playerInterface.SyncRenderer();
            return;
        }
        LocalPlayer();
    }


    void LocalPlayer()
    {
        if (playerInterface.IsPlayerDead())
        {
            return;
        }

        float xInput = Input.GetAxis("Horizontal") * Time.deltaTime;
        if (xInput != 0)
        {
            playerInterface.Move(xInput);
        }
        else
        {
            playerInterface.StopMove();
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            playerInterface.Jump();
        }

        if (Input.GetButtonDown("Punch"))
        {
            playerInterface.Punch();
        }
    }
}
