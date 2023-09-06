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
    //客户端Socket及状态信息
    static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

    //读取Listenfd
    public static void ReadListenfd(Socket listenfd) 
    {
        Console.WriteLine("Accept");

        Socket clientfd = listenfd.Accept();

        //获取客户端Socket并添加至clients Dictionary
        ClientState state = new ClientState();
        state.socket = clientfd;
        clients.Add(clientfd, state);

    }

    //读取Clientfd
    public static bool ReadClientfd(Socket clientfd) 
    {
        ClientState state = clients[clientfd];
        //接收
        int count = 0;
        try
        {
            count = clientfd.Receive(state.readBuff);

        }
        catch(SocketException ex) 
        {
            clientfd.Close();
            clients.Remove(clientfd);
            Console.WriteLine("Receive SocketException" + ex.ToString());
            return false;
            
        }

        //客户端关闭
        if (count == 0) 
        {
            clientfd.Close();
            clients.Remove(clientfd);
            Console.WriteLine("SocketClose");
                return false;
        }

        //广播
        string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
        Console.WriteLine("Receive:" + recvStr);
        string sendStr = clientfd.RemoteEndPoint.ToString() + ':' + recvStr;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        foreach (ClientState cs in clients.Values) 
        {
            cs.socket.Send(sendBytes);
        }
        return true;


    }


    public static void Main(string[] args) 
    {
        //Socket
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Bind
        IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipEp = new IPEndPoint(ipAdr, 1234);
        listenfd.Bind(ipEp);

        //Listen
        listenfd.Listen(0);
        Console.WriteLine("ServerStartSucessfully");

        //checkList
        List<Socket> checkList = new List<Socket>();

        //main loop
        while (true) 
        {
            //填充checkList
            checkList.Clear();
            checkList.Add(listenfd);
            foreach (ClientState s in clients.Values)
            {
                checkList.Add(s.socket);
            }

            //Select
            Socket.Select(checkList, null, null, 1000);

            //检查可读对象
            foreach (Socket s in checkList) 
            {
                if (s == listenfd)
                {
                    ReadListenfd(s);
                }

                else 
                {
                    ReadClientfd(s);
                }
            }


        }

    }

}
