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
    public static void UpdatePositionUnit(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        int _unitId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameObject unit = GetUnit(_playerId, _unitId);

        if (!(unit is null))
        {
            unit.transform.position = _position;
            unit.transform.rotation = _rotation;
        }

    }
    
    public static void UpdateHealthPointsUnit(Packet _packet)
    {
        int _playerId = _packet.ReadInt();
        int _unitId = _packet.ReadInt();
        int _healthPoints = _packet.ReadInt();

        GameObject unit = GetUnit(_playerId, _unitId);

        if (!(unit is null))
        {
            unit.GetComponent<ObjectInfo>().currentHealth = _healthPoints;
        }

    }

    public static void UnitAttack(Packet _packet)
    {

        int _playerId = _packet.ReadInt();
        int _unitId = _packet.ReadInt();        
        int _targetPlayerId = _packet.ReadInt();
        int _targetUnitId = _packet.ReadInt();

        GameObject _unit = GetUnit(_playerId, _unitId);
        GameObject _target = GetUnitIncludingLocalUnits(_targetPlayerId, _targetUnitId);

        if (!(_unit is null) && !(_target is null))
        {
            _unit.GetComponent<ObjectInfo>().Attack(_target, false);
        }

    }

    private static GameObject GetUnit(int _playerId, int _unitId) 
        => GameManager.players[_playerId].units
        .Where(u =>
        u.GetComponent<ObjectInfo>().id == _unitId &&
        u.GetComponent<ObjectInfo>().isLocalPlayerUnit != true)
        .FirstOrDefault();
    
    private static GameObject GetUnitIncludingLocalUnits(int _playerId, int _unitId) 
        => GameManager.players[_playerId].units
        .Where(u =>
        u.GetComponent<ObjectInfo>().id == _unitId)
        .FirstOrDefault();

}
