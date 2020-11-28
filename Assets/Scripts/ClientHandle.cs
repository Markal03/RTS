using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        ClientConnection.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        ClientConnection.instance.udp.Connect(((IPEndPoint)ClientConnection.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void UDPTest(Packet _packet)
    {
        string _msg = _packet.ReadString();
        Debug.Log($"Received packet via UDP. Contians message: {_msg}");
        ClientSend.UDPTestReceived();
    }
}
