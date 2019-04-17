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
        //GetComponent<AudioSource>().playOnAwake = false;
       // GetComponent<AudioSource>().clip = sound;
       GetComponent<Renderer>().material.color = enemyColor;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {

        //GetComponent<AudioSource>().Play();
        GameObject hit = collision.gameObject;
        /*
        Health health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(10);
        }
        */
        //if (hit.name == "Player0" || hit.name == "Player1" || hit.name == "Player2")
       
            //hit.gameObject.transform.SetParent(hit.transform);
            print("hit something");
            //transform.parent = collision.transform.parent;
            hit.GetComponent<PlayerController>().TakeColour(enemyColor);
            hit.GetComponent<PlayerController>().onlyOnce = true;
        Debug.Log("taking this color :" + enemyColor);
       
        Destroy(gameObject);
        
    }
}
