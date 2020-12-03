using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Linq;

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

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void SpawnUnit(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        int _unitId = _packet.ReadInt();
        int _health = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnUnit(_playerId, _unitId, _health, _position, _rotation);
        
        
    }
    public static void UpdateUnit(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        int _unitId = _packet.ReadInt();
        int _health = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameObject unit = GameManager.players[_playerId].units
            .Where(u => u.GetComponent<ObjectInfo>().id == _unitId)
            .FirstOrDefault();

        unit.GetComponent<ObjectInfo>().currentHealth = _health;
        unit.transform.position = _position;
        unit.transform.rotation = _rotation;
    }
}
