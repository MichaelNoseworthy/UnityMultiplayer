using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public AudioClip sound;

    // Use this for initialization
    void Start()
    {
        GetComponent<AudioSource>().playOnAwake = false;
        GetComponent<AudioSource>().clip = sound;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {

        GetComponent<AudioSource>().Play();
        GameObject hit = collision.gameObject;
        /*
        Health health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(10);
        }
        */
        this.gameObject.transform.SetParent(hit.transform);
        Destroy(gameObject, 1);
        
    }
}
