using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TransformMessage : MessageBase
{

    public static short MsgId = MsgType.Highest + 1;
    public static float SendTime = 0.5f;

    //This function will send messages to the console if a Transform message is successful
    public TransformMessage()
    {
        Debug.Log("Transform Message Sent");
    }

    public Vector3 position = Vector3.zero; //Set pos to zero
    public NetworkInstanceId netId; //public netId used for creating a network

}
