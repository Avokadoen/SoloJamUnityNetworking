using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetManager : NetworkManager
{

    public Transform[] spawnPosition;
    public int curPlayer;


    //Called on client when connect
    public override void OnClientConnect(NetworkConnection conn)
    {
        NetworkStartPosition[] spawnPositionArray = FindObjectsOfType<NetworkStartPosition>();
        if(spawnPositionArray.Length > 1)
        {
            spawnPosition = new Transform[spawnPositionArray.Length];
            int i = 0;
            foreach (var transform in spawnPositionArray)
            {
                spawnPosition[i] = spawnPositionArray[i++].transform;
            }
                
        }
        else
        {
            spawnPosition = new Transform[1];
            Debug.Log("no spawn positions");
            spawnPosition[0].position = new Vector3(0, 0, 0);
        }  

        // Create message to set the player
        IntegerMessage msg = new IntegerMessage(curPlayer);

        // Call Add player and pass the message
        ClientScene.AddPlayer(conn, 0, msg);
    }

    // Server
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        curPlayer = 0;
        // Read client message and receive index
        if (extraMessageReader != null)
        {
            var stream = extraMessageReader.ReadMessage<IntegerMessage>();
            curPlayer = stream.value;
        }
        //Select the prefab from the spawnable objects list
        var playerPrefab = spawnPrefabs[curPlayer];

        // Create player object with prefab
        var player = Instantiate(playerPrefab, spawnPosition[Random.Range(0, spawnPosition.Length)].position, Quaternion.identity) as GameObject;

        // Add player object for connection
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}