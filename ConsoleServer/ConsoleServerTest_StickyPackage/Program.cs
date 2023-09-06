using System;
using System.Net;
using System.Net.Sockets;
namespace EchoServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("ServerStart");

            //Socket
            Socket listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 1234);
            listenfd.Bind(ipEp);

            //Listen
            listenfd.Listen(0);
            Console.WriteLine("ServerStrartSuccessfully!!");

            //Accept
            Socket connfd = listenfd.Accept();
            Console.WriteLine("ServerAccept");

            //等待
            System.Threading.Thread.Sleep(30 * 1000);
            //Receive
            byte[] readBuff = new byte[1024];
            int count = connfd.Receive(readBuff);
            string readStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            Console.WriteLine("ServerReceive:" + readStr);






        }

    }
}

