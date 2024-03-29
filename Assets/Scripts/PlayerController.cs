﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : NetworkBehaviour
{

    public GameObject pickup = null;
    public bool m_canAttack = false;

    public GameObject Bullet1;
    public GameObject Bullet2;
    public GameObject Bullet3;

    //player
    private Rigidbody m_rb = null;
    private float m_speed = 5.0f;
    public Transform m_glassesTransform;
    public Transform spawnPoint;
    public GameObject m_glassesPrefab;
    private int ammo = 0;
    private static bool isFireEnabled = true;
    private float bulletSpeed = 20;
    public Text redScore;
    public Text blueScore;
    public Text greenScore;
    public Text AmmoAmount;
    public GameObject GameOverPanel;
    public bool wasHit = false;
    public bool hitOnlyOnce = true;
    bool doOnce = true;
    public bool PauseEnabled = false;
    public GameObject PauseMenuUI;
    public Text winnerText;
    public float timerEngaged = -1;
    public bool canMove = false;
    public Text InGameTimer;
    public GameObject GameTimerPanel;
    public Text ShowAmmo;



    //camera
    Camera cam;
    Camera MainCamera;
    Canvas canvas;
    Canvas MainCameraCanvas;
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
        //test
        canMove = false;
        MainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        cam = GetComponentInChildren<Camera>();
        cam.enabled = false;
        MainCamera.enabled = true;
        Cursor.visible = false;

        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = hasAuthority;

        m_rb = GetComponent<Rigidbody>();
        transform.position = GameObject.Find("SpawnPointsScript").GetComponent<PlayersSpawn>().GetSpawnPoint();
        PauseMenuUI.SetActive(false);
        GameTimerPanel.SetActive(false);

        GameOverPanel.SetActive(false);

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

        if (hasAuthority && isServer && GameManager.players >= 1 && doOnce == true)
        {
            /*
            Vector3 spawnPos = GameObject.Find("PickupSpawnPoint1").transform.position;
            GameObject _pickup = Instantiate(pickup, spawnPos, transform.rotation);
            NetworkServer.Spawn(_pickup);
            doOnce = false;
            */
        }
        //camera

        if (GameManager.GameStarted == true && GameManager.players < 4)
        {
            
            MainCamera.gameObject.SetActive(false);
            cam.enabled = true;
            cam.gameObject.SetActive(true);
            GameTimerPanel.SetActive(true);
            InGameTimer.text = GameManager.currentInGameTime;
        }
        if (GameManager.GameStarted == true && GameManager.players >= 4)
        {
            GameObject Player0 = GameObject.Find("Player0");
            canvas = Player0.GetComponentInChildren<Canvas>();
            canvas.enabled = true;
        }

            if (isLocalPlayer && GameManager.GameStarted == true)
        {
            cam.enabled = true;
            yaw += speedH * Input.GetAxis("Mouse X");
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

        if (!GameManager.GameStarted)
        {
            ShowAmmo.enabled = false;
            AmmoAmount.text = "";
        }
        if (GameManager.GameStarted)
        {
            ShowAmmo.enabled = true;
            AmmoAmount.text = ammo.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Cursor.visible = !Cursor.visible;
            PauseEnabled = !PauseEnabled;
            PauseMenuUI.SetActive(PauseEnabled);
        }

        CmdgetScoreUpdate();
        getScoreUpdate();

        if (GameManager.gameWon == true)
        {
            turnCursorOn();
        }


        Vector3 forward = Input.GetAxis("Vertical") * transform.right * m_speed;
        Vector3 strafe = Input.GetAxis("Horizontal") * transform.forward * -m_speed;
        m_rb.velocity = forward + strafe;
                
        if (m_updateTimePosition > m_updateTimePositionRate)
        {
            SendPosition();
            m_updateTimePosition = 0;
        }

        if (wasHit && hitOnlyOnce) {
            timerEngaged = 0;
            hitOnlyOnce = false;
        }


        if (wasHit)
        {
           
            GetComponent<MeshRenderer>().material.color = enemyColor;
            if (Time.fixedTime % .5 < .2)
                 {
                    GetComponent<MeshRenderer>().material.color = thisColor;
                    RpcSwitchThisColor();
                 }
                 else
                 {
                    GetComponent<MeshRenderer>().material.color = enemyColor;
                    RpcSwitchEnemyColor();
                }
                
        }

        timerEngaged += Time.deltaTime;

        if (timerEngaged >= 3)
        {
            wasHit = false;
            hitOnlyOnce = true;
            GetComponent<MeshRenderer>().material.color = thisColor;
            RpcSwitchThisColor();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            RpcSetTimeLower();
        }
    }

    [Command]
    public void CmdSpawnGlass() {

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
    }
    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "BulletGreen")
        {
            RpcsetGreenScore();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "BulletBlue")
        {
            RpcsetBlueScore();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "BulletRed")
        {
            RpcsetRedScore();
            Destroy(collision.gameObject);
        }
    }
    [Command]
    void CmdgetScoreUpdate()
    {
        

        redScore.text = GameManager.rScore.ToString();

        greenScore.text = GameManager.gScore.ToString();

        blueScore.text = GameManager.bScore.ToString();
    }

    void getScoreUpdate()
    {
        

        redScore.text = GameManager.rScore.ToString();

        greenScore.text = GameManager.gScore.ToString();

        blueScore.text = GameManager.bScore.ToString();
    }

    [ClientRpc]
    public void RpcTakeColour(Color color)
    {
        enemyColor = color;
        Debug.Log("receiving this color :" + enemyColor);
        wasHit = true;
        hitOnlyOnce = true;
    }

    [ClientRpc]
    void RpcsetGreenScore()
    {
        GameManager.gScore += 1;
    }

    [ClientRpc]
    void RpcsetBlueScore()
    {
        GameManager.bScore += 1;
    }

    [ClientRpc]
    void RpcsetRedScore()
    {
        GameManager.rScore += 1;
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
		Application.Quit();
        #endif
    }

    public void ResumeButton()
    {
        Cursor.visible = !Cursor.visible;
        PauseEnabled = !PauseEnabled;
        PauseMenuUI.SetActive(PauseEnabled);
    }
    [ClientRpc]
    public void RpcsetWinnerText()
    {
        winnerText.text = "Testing";
    }


    


    [ClientRpc]
    public void RpcSwitchThisColor()
    {
        GetComponent<MeshRenderer>().material.color = thisColor;
    }

    [ClientRpc]
    public void RpcSwitchEnemyColor()
    {
        GetComponent<MeshRenderer>().material.color = enemyColor;
    }

    public void turnCursorOn()
    {
        Cursor.visible = true;
        GameOverPanel.SetActive(true);
        winnerText.text = GameManager.Winner;
        turnCursorOn();
        Time.timeScale = 0;
    }

    [ClientRpc]
    public void RpcSetTimeLower()
    {
        GameManager.inGameTimer = 20;
    }

}