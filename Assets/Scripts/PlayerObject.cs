using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour {

    public GameObject playerIns;
    private Rigidbody m_rb = null;
    public float m_speed = 5.0f;

    void Start () {

        if (isLocalPlayer == false)
        {
            // This object belongs to another player.
            return;
        }

        CmdSpawnMyUnity();
      //  

    }
	
	// Update is called once per frame
	void Update () {
        if (isLocalPlayer == false)
        {
            return;
        }

    }

    [Command]
    void CmdSpawnMyUnity()
    {
        Destroy(this, 5);
        if (FindObjectOfType<PlayerController>() != null)
        {
            //spawn in a different place i ncase there is already a player
            GameObject go = Instantiate(playerIns);
            go.AddComponent<Rigidbody>();

            //go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
            m_rb = go.GetComponent<Rigidbody>();
            go.transform.position = new Vector3(0, 1, 0);
            NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
        }
        else {
            Debug.Log("this isnt null");
            GameObject go = Instantiate(playerIns);
            go.AddComponent<Rigidbody>();            
            m_rb = go.GetComponent<Rigidbody>();
            NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
        }
        
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("OnStartServer");
    }
}
