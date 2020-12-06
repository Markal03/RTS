using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        ClientConnection.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        ClientConnection.instance.udp.SendData(_packet);

    }
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(ClientConnection.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void UnitPositionUpdate(int unitId, Vector3 position, Quaternion rotation)
    {
        using (Packet _packet = new Packet((int)ClientPackets.unitPositionUpdate))
        {
            _packet.Write(unitId);
            _packet.Write(position);
            _packet.Write(rotation);
            _packet.Write(Time.time);
            SendUDPData(_packet);
        }
    }
    
    public static void UnitHealthPointsUpdate(int _unitId, int _currentHealthPoints)
    {
        using (Packet _packet = new Packet((int)ClientPackets.unitHPUpdate))
        {
            _packet.Write(_unitId);
            _packet.Write(_currentHealthPoints);
            SendTCPData(_packet);
        }
    }    
    public static void UnitAttack(int _unitId, int _targetId, int _targetPlayerId, int _attackDamage)
    {
        using (Packet _packet = new Packet((int)ClientPackets.unitAttack))
        {
            _packet.Write(_unitId);
            _packet.Write(_targetId);
            _packet.Write(_targetPlayerId);
            _packet.Write(_attackDamage);
            SendTCPData(_packet);
        }
    }
}
