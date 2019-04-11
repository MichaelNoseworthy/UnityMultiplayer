using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayersSpawn : NetworkBehaviour {

    public Transform playerSpawnPoint1;
    public Transform playerSpawnPoint2;
    public Transform playerSpawnPoint3;
    public Transform defaultSpawnPoint;
    bool playerSpawnPointinUse1 = false;
    bool playerSpawnPointinUse2 = false;
    bool playerSpawnPointinUse3 = false;

    GameObject[] SpawnPoints;

    // Use this for initialization
    void Start () {
	    SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        

        if (SpawnPoints[0])
            playerSpawnPoint1 = SpawnPoints[0].transform;
        if (SpawnPoints[1])
            playerSpawnPoint2 = SpawnPoints[1].transform;
        if (SpawnPoints[2])
            playerSpawnPoint3 = SpawnPoints[2].transform;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 GetSpawnPoint()
    {
        if (playerSpawnPointinUse1 == false)
        {
            playerSpawnPointinUse1 = true;
            return playerSpawnPoint1.position;
        }
        if (playerSpawnPointinUse2 == false)
        {
            playerSpawnPointinUse2 = true;
            return playerSpawnPoint2.position;
        }
        if (playerSpawnPointinUse3 == false)
        {
            playerSpawnPointinUse3 = true;
            return playerSpawnPoint3.position;
        }
        else
        {
            return defaultSpawnPoint.position;
        }
    }
}
