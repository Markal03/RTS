using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

public class ClientConnection : MonoBehaviour
{

    public static ClientConnection instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;

    public TCP tcp;
    public UDP udp;

    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;
    /**
     * @Awake()
     * Makes sure that only one instance of the client is connected
     **/
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destrying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    public void ConnectToServer()
    {
        InitialiseClientData();
        tcp.Connect();
    }
    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient()
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);

        }

        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            } 
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving TCP data: {_ex}");
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch(Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                HandleData(_data);
            }
            catch
            {

            }
        }

        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                packetHandlers[_packetId](_packet);
                }
            });
        }
    }
    private void InitialiseClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int) ServerPackets.welcome, ClientHandle.Welcome },
            { (int) ServerPackets.udpTest, ClientHandle.UDPTest }
        };

        Debug.Log("Initialised client data");
    }
}

//    public int id;

//    private string currentTime;
//    readonly UDPSocket c = new UDPSocket();
//    private void Start()
//    {
//        //c.Client("127.0.0.1", 27000);
//        //InvokeRepeating("SendPositionUpdates", 0.1f, 0.3f);
//    }

//    public void SendPositionUpdates(GameObject movingObject)
//    {
//        string position = movingObject.transform.position.ToString();
//        currentTime = Time.time.ToString("f6");
//        currentTime = "Time is: " + currentTime + " sec.";
//        c.Send(currentTime + position);
//    }

//    private void Update()
//    {
//        //string position = gameObject.transform.position.ToString();
//        //c.Send(position);
//        //SendPositionUpdates();
//    }

//}

//public class TCP
//{

//    public static int dataBufferSize = 4096;
//    public TcpClient socket;
//    private NetworkStream stream;
//    private byte[] receiveBuffer;

//    private readonly int id;

//    public TCP(int _id)
//    {
//        id = _id;
//    }

//    public void Connect(TcpClient _socket)
//    {
//        socket = _socket;
//        socket.ReceiveBufferSize = dataBufferSize;
//        socket.SendBufferSize = dataBufferSize;

//        stream = socket.GetStream();

//        receiveBuffer = new byte[dataBufferSize];

//        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
//    }

//    private void ReceiveCallback(IAsyncResult _result)
//    {
//        try
//        {
//            int _byteLength = stream.EndRead(_result);
//            if (_byteLength <= 0)
//            {
//                return;
//            }

//            byte[] _data = new byte[_byteLength];
//            Array.Copy(receiveBuffer, _data, _byteLength);

//            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

//        }
//        catch(Exception _ex)
//        {
//            Console.WriteLine($"Error receiving TCP data: {_ex}");
//        }
//    }
//}
//public class UDPSocket
//{
//    private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//    private const int bufSize = 8 * 1024;
//    private State state = new State();
//    private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
//    private AsyncCallback recv = null;

//    public class State
//    {
//        public byte[] buffer = new byte[bufSize];
//    }

//    public void Client(string address, int port)
//    {
//        _socket.Connect(IPAddress.Parse(address), port);
//        Receive();
//    }

//    public void Send(string text)
//    {
//         byte[] data = Encoding.ASCII.GetBytes(text);
//        _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
//        {
//            State so = (State)ar.AsyncState;
//            int bytes = _socket.EndSend(ar);
//            Console.WriteLine("SEND: {0}, {1}", bytes, text);
//        }, state);
//    }

//    private void Receive()
//    {
//        _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
//        {
//            State so = (State)ar.AsyncState;
//            int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
//            _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
//            Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
//        }, state);
//    }
//}