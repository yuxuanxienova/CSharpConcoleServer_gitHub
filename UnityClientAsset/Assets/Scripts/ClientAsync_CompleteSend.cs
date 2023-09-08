using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using TMPro;
using System.Linq;

public class ClientAsync_CompleteSend : MonoBehaviour
{
    Socket socket;
    public TMP_InputField tMP_InputField;
    public TMP_Text tMP_Text;


    //���ջ�����
    byte[] readBuff = new byte[1024];

    //���ջ����������ݳ���
    int buffCount = 0;

    string recvStr = " ";

    //������Ӱ�ť
    public void Connection()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //Async Connect
        // socket.BeginConnect("127.0.0.1", 1234, ConnectCallBack, socket);

        //������룺ʹ��ͬ��connect
        socket.Connect("127.0.0.1", 1234);
        socket.BeginReceive(readBuff, buffCount, 1024 - buffCount, 0, ReceiveCallBack, socket);


    }

    //Receive�ص�
    public void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;

            //��ȡ�������ݳ���
            int count = socket.EndReceive(ar);
            buffCount += count;

            //�����������Ϣ
            OnReceiveData();

            ////---------------ģ��ճ��--------------
            ////�ȴ�
            //System.Threading.Thread.Sleep(1000 * 30);
            ////-------------------------------------

            //�����������ݰ�
            socket.BeginReceive(readBuff, buffCount, 1024 - buffCount, 0, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive Fail");
            Debug.Log(ex);
        }

    }

    public void OnReceiveData()
    {
        Debug.Log("[Recv 1] buff Count = " + buffCount);
        Debug.Log("[Recv 2] readbuff = " + BitConverter.ToString(readBuff));

        //��Ϣ����
        //1. �������С����Ϣͷ�ĳ��ȣ�2����������
        if (buffCount <= 2)
        {
            return;
        }

        //���ڶ��Ĳ�������Ϣ��
        Int16 bodyLength = BitConverter.ToInt16(readBuff, 0);
        Debug.Log("[Recv 3] bodyLength = " + bodyLength);

        //2. �������С����Ϣͷ����Ϣ�壬������
        if (buffCount < 2 + bodyLength)
        {
            return;
        }

        //3. ���������ȴ���һ��������Ϣ
        string s = System.Text.Encoding.UTF8.GetString(readBuff, 2, buffCount);
        Debug.Log("[Recv 4] s = " + s);

        //3.1 ���»�����
        int start = 2 + bodyLength;
        int count = buffCount - start;
        Array.Copy(readBuff, start, readBuff, 0, count);
        buffCount -= start;

        Debug.Log("[Recv 5] buffCount = " + buffCount);

        //��Ϣ����
        recvStr = s + "\n" + recvStr;

        //������ȡ��Ϣ
        OnReceiveData();




    }

    // //Connect �ص�
    // public void ConnectCallBack(IAsyncResult ar)
    // {
    //     try
    //     {
    //         Socket socket = (Socket) ar.AsyncState;
    //         socket.EndConnect(ar);
    //         Debug.Log("Socket Connect Succ");

    //         //Receive the message
    //         socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);

    //     }
    //     catch (SocketException ex)
    //     {
    //         Debug.Log("Socket Connect Faill" + ex.ToString());
    //     }
    // }



    //-------------------------��������-------------------------------------
    //����
    Queue<ByteArray> writeQueue = new Queue<ByteArray>();

    //������Ͱ�ť   
    public void Send()
    {
        //Send
        string sendStr = tMP_InputField.text;

        //��װЭ��
        byte[] bodyBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        Int16 len = (Int16)bodyBytes.Length;
        byte[] lenBytes = BitConverter.GetBytes(len);
        byte[] sendBytes = lenBytes.Concat(bodyBytes).ToArray();

        //---------------------------------------------------

        //��ʼ��ByteArray Ȼ�� ���
        ByteArray ba = new ByteArray(sendBytes);
        writeQueue.Enqueue(ba);

        //������ֻ��һ��������Ϣʱ��ŷ���
        if (writeQueue.Count == 1) 
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);

        }

        //-----------------------------------------------------

    }


    public void SendCallback(IAsyncResult ar)
    {
        //��ȡstate, ��ȡEndSend �Ĵ���
        Socket socket = (Socket)ar.AsyncState;
        int count = socket.EndSend(ar);

        //�ж����������
        ByteArray ba = writeQueue.First();
        ba.readIdx += count;
        if (ba.length == 0) 
        {
            writeQueue.Dequeue();
            ba = writeQueue.First();
        }

        //�����Ϣ���Ͳ�����������ڵڶ�������
        if (ba != null) 
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }


    }
    //---------------------------------------------------------------------------------

    public void Update()
    {
        tMP_Text.text = recvStr;
    }

}
