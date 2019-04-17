using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : NetworkBehaviour
{

    public GameObject pickup = null;
    public bool m_canAttack = false;
    public float attackCooldownTime = 10.0f;
    private float currentAttackTimer = -1.0f;

    public GameObject Bullet1;
    public GameObject Bullet2;
    public GameObject Bullet3;

    //player
    private Rigidbody m_rb = null;
    public float m_speed = 5.0f;
    public Transform m_glassesTransform;
    public Transform spawnPoint;
    public GameObject m_glassesPrefab;
    public int ammo = 5;
    public static bool isFireEnabled = true;
    private float bulletSpeed = 6;
    public Text redScore;
    public Text blueScore;
    public Text greenScore;
    public bool beenHit = false;
    bool doOnce = true;


    //camera
    Camera cam;
    Canvas canvas;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private float m_transformMsgTimer = 2.0f;
    public float m_updateTimePosition = 0.0f;
    public float m_updateTimePositionRate = 0.5f;

    [SyncVar]
    public Color thisColor;
    [SyncVar]
    public Color enemyColor;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        cam.enabled = hasAuthority;
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = hasAuthority;


        m_rb = GetComponent<Rigidbody>();
        transform.position = GameObject.Find("SpawnPointsScript").GetComponent<PlayersSpawn>().GetSpawnPoint();


        if (isServer)
        {
            NetworkServer.RegisterHandler(TransformMessage.MsgId, OnTransformMsg);
            CustomNetworkManager.singleton.client.RegisterHandler(TransformMessage.MsgId, OnTransformMsg);
        }

    }

     protected void OnGameStateMsg(NetworkMessage netMsg)
    {
        GameStateMsg msg = netMsg.ReadMessage<GameStateMsg>();
        Debug.Log("OnGameStateMsg msg.m_netId: " + msg.m_netId);
        Debug.Log("OnGameStateMsg netId: " + netId);
        //sent from other machine
        //net ids match, update this replica
        if (msg.m_netId == netId)
        {
            this.transform.position = msg.m_position;
            Debug.Log("OnGameStateMsg Position: " + msg.m_position);
        }
        
    }


    protected void OnTransformMsg(NetworkMessage msg)
    {
        TransformMessage transformMsg = msg.ReadMessage<TransformMessage>();

        Debug.Log("OnTransformMsg: NetId: " + netId);
        Debug.Log("OnTransformMsg: MsgNetId: " + transformMsg.netId);
        Debug.Log("OnTransformMsg: Position: " + transformMsg.position);

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

        Debug.Log("SendTransformMsg: netId: " + netId + " isSuccess: " + isSuccess);
    }

    public void SendPosition()
    {
        GameStateMsg msg = new GameStateMsg();
        msg.m_netId = netId;
        msg.m_position = transform.position;
        bool sendResult = false;
        if (isServer)
        {
            sendResult = NetworkServer.SendToAll(GameStateMsg.msgId, msg);

        }
        else
        {
            sendResult = NetworkManager.singleton.client.Send(GameStateMsg.msgId, msg);
        }

        if (sendResult)
        {
            Debug.Log("Sending msg");
        }
        else
        {
            Debug.Log("Failed Sending msg");
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_updateTimePosition += Time.deltaTime;
        if (!hasAuthority)
        {
            return;
        }

        if (hasAuthority && isServer && GameManager.players >= 3 && doOnce == true)
        {
            Vector3 spawnPos = GameObject.Find("PickupSpawnPoint1").transform.position;
            GameObject _pickup = Instantiate(pickup, spawnPos, transform.rotation);

            //spawn pickup
            NetworkServer.Spawn(_pickup);
            //attachCamera()
            doOnce = false;
        }
        //camera
        if (!isLocalPlayer)
        {

            cam.enabled = false;
            return;
        }
        if (isLocalPlayer)
        {
            cam.enabled = true;
            yaw += speedH * Input.GetAxis("Mouse X");
            //pitch -= speedV * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (ammo > 0)
            {
                CmdSpawnGlass();
                ammo--;
            }
            else
            {
                print("OUT OF AMMO");
            }
            
        }

        CmdgetScoreUpdate();
        getScoreUpdate();


        Vector3 forward = Input.GetAxis("Vertical") * transform.right * m_speed;
        Vector3 strafe = Input.GetAxis("Horizontal") * transform.forward * -m_speed;
        m_rb.velocity = forward + strafe;
                
        if (m_updateTimePosition > m_updateTimePositionRate)
        {
            SendPosition();
            m_updateTimePosition = 0;
        }

        /*
        if (beenHit)
        {
            float timerEngaged = 0;
            GetComponent<MeshRenderer>().material.color = enemyColor;
            while (beenHit)
            {

                timerEngaged *= Time.deltaTime;

                if (timerEngaged % 0.2 == 0)
                {
                    GetComponent<MeshRenderer>().material.color = thisColor;
                }
                else
                {
                    GetComponent<MeshRenderer>().material.color = enemyColor;
                }

                if (timerEngaged >= 3)
                {
                    beenHit = false;
                    GetComponent<MeshRenderer>().material.color = thisColor;
                    break;
                }

            }
        }
        */
    }

    [Command]
    public void CmdSpawnGlass() {
        /*
        GameObject glassesProjectile = Instantiate(m_glassesPrefab, m_glassesTransform);
        glassesProjectile.GetComponent<MeshRenderer>().material.color = thisColor;
        Rigidbody rb = glassesProjectile.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = glassesProjectile.transform.right * m_speed;

        NetworkServer.Spawn(glassesProjectile);
        Destroy(glassesProjectile, 0.5f);
        */
        if (isFireEnabled)
        {
            if (this.name == "Player0")
            {
                var bullet1 = (GameObject)Instantiate(Bullet1,
                                                     m_glassesPrefab.transform.position,
                                                     m_glassesPrefab.transform.rotation);
                bullet1.GetComponent<MeshRenderer>().material.color = thisColor;
                bullet1.GetComponent<Rigidbody>().velocity = bullet1.transform.forward * bulletSpeed;
                bullet1.GetComponent<Projectile>().enemyColor = thisColor;
                NetworkServer.Spawn(bullet1);
                Destroy(bullet1, 5);
            }
            else if (this.name == "Player1")
            {
                var bullet2 = (GameObject)Instantiate(Bullet2,
                                                     m_glassesPrefab.transform.position,
                                                     m_glassesPrefab.transform.rotation);
                bullet2.GetComponent<MeshRenderer>().material.color = thisColor;
                bullet2.GetComponent<Rigidbody>().velocity = bullet2.transform.forward * bulletSpeed;
                bullet2.GetComponent<Projectile>().enemyColor = thisColor;
                NetworkServer.Spawn(bullet2);
                Destroy(bullet2, 5);
            }
            else if (this.name == "Player2")
            {
                var bullet3 = (GameObject)Instantiate(Bullet3,
                                                     m_glassesPrefab.transform.position,
                                                     m_glassesPrefab.transform.rotation);
                bullet3.GetComponent<MeshRenderer>().material.color = thisColor;
                bullet3.GetComponent<Rigidbody>().velocity = bullet3.transform.forward * bulletSpeed;
                bullet3.GetComponent<Projectile>().enemyColor = thisColor;
                NetworkServer.Spawn(bullet3);
                Destroy(bullet3, 5);
            }
        }
    }

    public override void OnStartClient()
    {
        GetComponent<MeshRenderer>().material.color = thisColor;
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Pickup"))
        {
            Debug.Log("Player collision w/ Pickup");
            ammo += 5;
            m_canAttack = true;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "BulletGreen")
        {
            GameManager.gScore += 1;
        }

        if (collision.gameObject.tag == "BulletBlue")
        {
            GameManager.bScore += 1;
        }

        if (collision.gameObject.tag == "BulletRed")
        {
            GameManager.rScore += 1;
        }
    }
    [Command]
    void CmdgetScoreUpdate()
    {

        //GameObject GameManager = GameObject.Find("GameManager");

        redScore.text = GameManager.rScore.ToString();

        greenScore.text = GameManager.gScore.ToString();

        blueScore.text = GameManager.bScore.ToString();
    }

    void getScoreUpdate()
    {

        //GameObject GameManager = GameObject.Find("GameManager");

        redScore.text = GameManager.rScore.ToString();

        greenScore.text = GameManager.gScore.ToString();

        blueScore.text = GameManager.bScore.ToString();
    }


    public void TakeColour(Color color)
    {
        enemyColor = color;
        beenHit = true;
    }
}