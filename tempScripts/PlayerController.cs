using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

    public GameObject bulletPrefabR;
    public GameObject bulletPrefabG;
    public GameObject bulletPrefabB;
    public Transform bulletSpawn;

    private float playerRotateSpeed = 150.0f;
    private float playerMoveSpeed = 3.0f;

    private float bulletSpeed = 6;
    private float bulletDeathTime = 2.0f;

    private float m_transformMsgTimer = 2.0f;

    [SyncVar]
    public Color color;

    int ammo = 10;

    public Camera camera;

    public static bool canFire = false;

    void Start()
    {
        NetworkServer.RegisterHandler(TransformMessage.MsgId, OnTransformMsg);
        CustomNetworkManager.singleton.client.RegisterHandler(TransformMessage.MsgId, OnTransformMsg);
        GetComponent<MeshRenderer>().material.color = color;
        if (isLocalPlayer) return;
        camera.enabled = false;
    }

    protected void OnTransformMsg(NetworkMessage msg)
    {
        TransformMessage transformMsg = msg.ReadMessage<TransformMessage>();

        //Debug.Log("OnTransformMsg: netId: " + netId);
        //Debug.Log("OnTransformMsg: msgNetId: " + transformMsg.netId);
        //Debug.Log("OnTransformMsg: position: " + transformMsg.position);

    }

    protected void SendTransformMsg()
    {
        TransformMessage transformMsg = new TransformMessage();
        transformMsg.netId = netId;
        transformMsg.position = transform.position;
        bool isSuccess = false;

        if (isServer) //If connected to server successfully, send messaage 
        {
            isSuccess = NetworkServer.SendToAll(TransformMessage.MsgId, transformMsg);
        }

        //Debug.Log("SendTransformMsg: netId: " + netId + " isSuccess: " + isSuccess);

    }

    void Update()
    {
            if (!isLocalPlayer)
            {
                return;
            }

            var x = Input.GetAxis("Horizontal") * Time.deltaTime * playerRotateSpeed;
            var z = Input.GetAxis("Vertical") * Time.deltaTime * playerMoveSpeed;

            transform.Rotate(0, x, 0);
            transform.Translate(0, 0, z);

            m_transformMsgTimer -= Time.deltaTime;
            if (m_transformMsgTimer < 0.0f)
            {
                SendTransformMsg();
                m_transformMsgTimer = TransformMessage.SendTime;
                m_transformMsgTimer = 2.0f;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (ammo > 0)
                {
                    CmdFire();
                    ammo--;
                }
                else
                {
                    print("OUT OF AMMO");
                }
            }

        

    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Ammo")
        {
            ammo += 10;
            print("Ammo Loaded: " + ammo);
            Destroy(other.gameObject);
        }

        if(other.gameObject.tag == "greenBullet")
        {
            GameManager.gScore += 1;
            print("greenPoint");
        }

        if (other.gameObject.tag == "blueBullet")
        {
            GameManager.bScore += 1;
            print("bluePoint");
        }

        if (other.gameObject.tag == "redBullet")
        {
            GameManager.rScore += 1;
            print("Red point!");
        }

    }

    [Command]
    private void CmdFire()
    {
        if (canFire)
        {
            if (this.name == "Player0")
            {
                var bullet1 = (GameObject)Instantiate(bulletPrefabR,
                                                     bulletSpawn.position,
                                                     bulletSpawn.rotation);
                bullet1.GetComponent<Rigidbody>().velocity = bullet1.transform.forward * bulletSpeed;
                NetworkServer.Spawn(bullet1);
                Destroy(bullet1, bulletDeathTime);
            }
            else if (this.name == "Player1")
            {
                var bullet2 = (GameObject)Instantiate(bulletPrefabG,
                                                     bulletSpawn.position,
                                                     bulletSpawn.rotation);
                bullet2.GetComponent<Rigidbody>().velocity = bullet2.transform.forward * bulletSpeed;
                NetworkServer.Spawn(bullet2);
                Destroy(bullet2, bulletDeathTime);
            }
            else if (this.name == "Player2")
            {
                var bullet3 = (GameObject)Instantiate(bulletPrefabB,
                                                     bulletSpawn.position,
                                                     bulletSpawn.rotation);
                bullet3.GetComponent<Rigidbody>().velocity = bullet3.transform.forward * bulletSpeed;
                NetworkServer.Spawn(bullet3);
                Destroy(bullet3, bulletDeathTime);
            }
        }
    }


}
