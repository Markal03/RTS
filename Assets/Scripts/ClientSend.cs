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

    public static void UnitUpdate(int unitId, int currentHealth, Vector3 position, Quaternion rotation)
    {
        using (Packet _packet = new Packet((int)ClientPackets.unitUpdate))
        {
            _packet.Write(unitId);
            _packet.Write(currentHealth);
            _packet.Write(position);
            _packet.Write(rotation);

            SendUDPData(_packet);
        }
    }
}
