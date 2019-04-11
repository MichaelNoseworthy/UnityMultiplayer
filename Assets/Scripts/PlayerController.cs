using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : NetworkBehaviour
{
    //player
    private Rigidbody m_rb = null;
    public float m_speed = 5.0f;
    public Transform m_glassesTransform;
    public Transform spawnPoint;
    public GameObject m_glassesPrefab;

    //camera
    Camera cam;
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

        if (isLocalPlayer)
        {
            m_rb = GetComponent<Rigidbody>();
            NetworkServer.RegisterHandler(TransformMessage.MsgId, OnTransformMsg);
            CustomNetworkManager.singleton.client.RegisterHandler(TransformMessage.MsgId, OnTransformMsg);
            cam = GetComponentInChildren<Camera>();

            transform.position = GameObject.Find("SpawnPointsScript").GetComponent<PlayersSpawn>().GetSpawnPoint();
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

        if (!isLocalPlayer)
        {
            cam.enabled = false;
            return;
        }

        //camera
        if (isLocalPlayer)
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            //pitch -= speedV * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            CmdSpawnGlass();
        }

        Vector3 forward = Input.GetAxis("Vertical") * transform.right * m_speed;
        Vector3 strafe = Input.GetAxis("Horizontal") * transform.forward * -m_speed;
        m_rb.velocity = forward + strafe;
                
        if (m_updateTimePosition > m_updateTimePositionRate)
        {
            SendPosition();
            m_updateTimePosition = 0;
        }
    }

    [Command]
    public void CmdSpawnGlass() {
        GameObject glassesProjectile = Instantiate(m_glassesPrefab, m_glassesTransform);
        glassesProjectile.GetComponent<MeshRenderer>().material.color = thisColor;
        Rigidbody rb = glassesProjectile.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = glassesProjectile.transform.right * m_speed;

        NetworkServer.Spawn(glassesProjectile);
        Destroy(glassesProjectile, 0.5f);
    }

    public override void OnStartClient()
    {
        GetComponent<MeshRenderer>().material.color = thisColor;
    }
}