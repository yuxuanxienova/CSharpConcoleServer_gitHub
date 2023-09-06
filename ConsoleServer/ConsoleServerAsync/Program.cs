using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

class ClientState 
{
    public Socket socket;
    public byte[] readBuff = new byte[1024];
}

class MainClass
{
    //监听Socket
    static Socket listenfd;

    //客户端Socket 及状态信息
    static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
    public static void Main(string[] args) 
    {
        Console.WriteLine("ServerStart");

        //Socket
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //Bind
        IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipEp = new IPEndPoint(ipAdr, 1234);
        listenfd.Bind(ipEp);

        //Listen
        listenfd.Listen(0);
        Console.WriteLine("ServerStartSuccessfully");

        //Accept
        listenfd.BeginAccept(AcceptCallback, listenfd);
        //等待
        Console.ReadLine();

    }

    //Accept回调
    public static void AcceptCallback(IAsyncResult ar) 
    {
        try
        {
            Console.WriteLine("ServerAccept");
            Socket listenfd = (Socket)ar.AsyncState;
            Socket clientfd = listenfd.EndAccept(ar);

            //写入client列表
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);

            //接收数据BeginReceive
            clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

            //继续Accept
            listenfd.BeginAccept(AcceptCallback, listenfd);

        }
        catch (SocketException ex) 
        {
            Console.WriteLine("Socket Accept Fail" + ex.ToString);
        }
    }

    //Receive回调
    public static void ReceiveCallback(IAsyncResult ar) 
    {
        try
        {
            ClientState state = (ClientState)ar.AsyncState;
            Socket clientfd = state.socket;
            int count = clientfd.EndReceive(ar);

            //客户端关闭
            if (count == 0)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("SocketClose");
                return;

            }

            string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);

            //SendBack
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes("echo" + recvStr);

            clientfd.Send(sendBytes);

            //继续调用receive
            clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

        }
        catch (SocketException ex)
        {
            Console.WriteLine("Socket Receive Fail" + ex.ToString);
        }
    }
    
}