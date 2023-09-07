using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;

public class ClientEcho : MonoBehaviour
{
    Socket socket;

    public TMP_InputField inputField;
    public TMP_Text text;

    //点击连接按钮
    public void Connection()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect
        socket.Connect("127.0.0.1" , 1234);        

    }

    //点击发送按钮
    public void Send()
    {
        //Send
        string sendStr = inputField.text;       
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);

        //Recv
        byte[] readBuff = new byte[1024];
        int count = socket.Receive(readBuff);
        string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
        text.text = recvStr;

        //Close
        // socket.Close();



    }

    public void CloseConnection()
    {
        socket.Close();
        Debug.Log("Connection Closed");

    }



}
