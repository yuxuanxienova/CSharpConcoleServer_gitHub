using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using TMPro;


public class ClientAsync : MonoBehaviour
{
    Socket socket;
    public TMP_InputField tMP_InputField;
    public TMP_Text tMP_Text;

    //接收缓冲区
    byte[] readBuff = new byte[1024];
    string recvStr = " ";

    //点击链接按钮
    public void Connection()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //Connect
        socket.BeginConnect("127.0.0.1", 1234, ConnectCallBack, socket);

    }

    //Receive回调
    public void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            int count = socket.EndReceive(ar);
            recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);

            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
        }
        catch(SocketException ex)
        {
            Debug.Log("Socket Receive Fail" );
            Debug.Log(ex);
        }

    }

    //Connect 回调
    public void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ");

            //Receive the message
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);

        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect Faill" + ex.ToString());
        }
    }



    //点击发送按钮
    public void Send()
    {
        //Send
        string sendStr = tMP_InputField.text;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);

    }

    public void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            int count = socket.EndSend(ar);
            Debug.Log("Socket Send succ" + count);

        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Send Fail" + ex.ToString());
        }

    }

    public void Update()
    {
        tMP_Text.text = recvStr;
    }
    //--------------------------Test Method-------------------
    // public void Send()
    // {
    //     string sendStr = tMP_InputField.text;
    //     byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
    //     for(int i=0; i<10000; i++)
    //     {
    //         socket.Send(sendBytes);
    //     }
    // }




}
