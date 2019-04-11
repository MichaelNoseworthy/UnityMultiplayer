using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameStateMsg : MessageBase
{
    //make sure to track all of your msg ids and not have any duplicate ids
    public static short msgId = MsgType.Highest + 1;
    public GameStateMsg()
    {
        m_position = Vector3.zero;
    }

    public Vector3 m_position;
    public NetworkInstanceId m_netId;
}
