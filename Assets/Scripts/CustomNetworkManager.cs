using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    public Color[] myColor;
    int counter = 0;
    //
    // Summary:
    //     Called on the client when connected to a server.
    //
    // Parameters:
    //   conn:
    //     Connection to the server.
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("OnClientConnect");
    }
    //
    // Summary:
    //     Called on clients when disconnected from a server.
    //
    // Parameters:
    //   conn:
    //     Connection to the server.
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Debug.Log("OnClientDisconnect");
    }
    
    
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        base.OnServerAddPlayer(conn, playerControllerId, extraMessageReader);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            player.GetComponent<MeshRenderer>().material.color = myColor[counter];
            player.GetComponent<PlayerController>().thisColor = myColor[counter];

        // Debug.Log("color:: " + colorcount);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);        
            counter++;                
    }

    //
    // Summary:
    //     Called on the server when a client adds a new player with ClientScene.AddPlayer.
    //
    // Parameters:
    //   conn:
    //     Connection from client.
    //
    //   playerControllerId:
    //     Id of the new player.
    //
    //   extraMessageReader:
    //     An extra message object passed for the new player.    

    //
    // Summary:
    //     Called on the server when a new client connects.
    //
    // Parameters:
    //   conn:
    //     Connection from client.
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect");
    }

    //
    // Summary:
    //     Called on the server when a client disconnects.
    //
    // Parameters:
    //   conn:
    //     Connection from client.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("OnServerDisconnect");
    }

    //
    // Summary:
    //     Called on the server when a network error occurs for a client connection.
    //
    // Parameters:
    //   conn:
    //     Connection from client.
    //
    //   errorCode:
    //     Error code.
    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        base.OnServerError(conn, errorCode);
    }

    //
    // Summary:
    //     Called on the server when a client is ready.
    //
    // Parameters:
    //   conn:
    //     Connection from client.
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
    }
    
    //
    // Summary:
    //     This is a hook that is invoked when the client is started.
    //
    // Parameters:
    //   client:
    //     The NetworkClient object that was started.
    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        Debug.Log("OnStartClient");
    }
    //
    // Summary:
    //     This hook is invoked when a host is started.
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("OnStartHost");
    }

    //
    // Summary:
    //     This hook is invoked when a server is started - including when a host is started.

}
