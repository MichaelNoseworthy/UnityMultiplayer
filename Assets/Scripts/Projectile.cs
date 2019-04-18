using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour
{

    public AudioClip sound;

    [SyncVar]
    public Color enemyColor;

    // Use this for initialization
    void Start()
    {
       GetComponent<Renderer>().material.color = enemyColor;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collision)
    {

        GameObject hit = collision.gameObject;
            print("hit something");
        if (hit.tag == "Player")
        {
            hit.GetComponent<PlayerController>().RpcTakeColour(enemyColor);
            Debug.Log("taking this color :" + enemyColor);
        }
       
        Destroy(gameObject);
        
    }
}
