/*using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Globalization;
using TMPro;
using Newtonsoft.Json;


#if !UNITY_EDITOR
using System.Threading.Tasks;
#endif

public class SocketConnectionWindows
{

#if !UNITY_EDITOR
    private bool _useUWP = true;
    private Windows.Networking.Sockets.StreamSocket socket;
    private Task exchangeTask, connectionTask;
#endif

#if UNITY_EDITOR
    private bool _useUWP = false;
    System.Net.Sockets.TcpClient client;
    System.Net.Sockets.NetworkStream stream;
    private Thread exchangeThread, connectionThread;
#endif

    private Byte[] bytes = new Byte[256];
    private StreamWriter writer;
    private BinaryWriter FileWriter;
    private StreamReader reader;
    private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
    private int id = 0;

    public string _host = "192.168.1.44";
    public string _port = "53000";

    const string splitter = "[{//V//}]";

    public SocketConnectionWindows()
    {
    }

    public void Init()
    {
        CreateClientThread();
    }

    public void CreateClientThread()
    {
#if UNITY_EDITOR
                if (connectionThread != null) StopExchange();
                connectionThread = new System.Threading.Thread(Connect);
                connectionThread.Start();
#else
        if (connectionTask != null) StopExchange();
        connectionTask = Task.Run(() => Connect());
#endif
    }

    public void Connect()
    {
        if (_useUWP)
        {
            ConnectUWP(_host, _port);
        }
        else
        {
            ConnectUnity(_host, _port);
        }
    }


#if UNITY_EDITOR
        private void ConnectUWP(string host, string port)
#else
    private async void ConnectUWP(string host, string port)
#endif
    {
#if UNITY_EDITOR
            errorStatus = "UWP TCP client used in Unity!";
#else
        try
        {
            if (exchangeTask != null) StopExchange();

            socket = new Windows.Networking.Sockets.StreamSocket();
            
            Windows.Networking.HostName serverHost = new Windows.Networking.HostName(host);

            await socket.ConnectAsync(serverHost, port);

            Stream streamOut = socket.OutputStream.AsStreamForWrite();
            writer = new StreamWriter(streamOut);

            Stream streamIn = socket.InputStream.AsStreamForRead();
            reader = new StreamReader(streamIn);

            StartExchange();
        }
        catch (Exception e)
        {
            //errorStatus = e.ToString();
            //ToConsole(errorStatus);


        }
#endif
    }


    private void ConnectUnity(string host, string port)
    {
#if !UNITY_EDITOR
        errorStatus = "Unity TCP client used in UWP!";
#else
        try
        {
            if (exchangeThread != null) StopExchange();

            client = new System.Net.Sockets.TcpClient(host, Int32.Parse(port));
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };
            FileWriter = new BinaryWriter(stream);
            StartExchange();
        }
        catch (Exception e)
        {
            errorStatus = e.ToString();
            //print(errorStatus);
        }
#endif
    }

    private bool exchanging = false;
    private bool exchangeStopRequested = false;
    private string lastPacket = null;

    private string errorStatus = null;
    private string warningStatus = null;
    private string successStatus = null;
    private string unknownStatus = null;

    public void StartExchange()
    {
#if UNITY_EDITOR
            if (exchangeThread != null) StopExchange();
            exchangeStopRequested = false;
            exchangeThread = new System.Threading.Thread(ReceivePackets);
            exchangeThread.Start();
#else
        if (exchangeTask != null) StopExchange();
        exchangeStopRequested = false;
        exchangeTask = Task.Run(() => ReceivePackets());
#endif

    }
#if UNITY_EDITOR
        private void ReceivePackets()
#else
    public void ReceivePackets()
#endif
    {

        while (!exchangeStopRequested)
        {
            if (reader == null) continue;
            exchanging = true;

            string received = null;

            try
            {
#if UNITY_EDITOR
                byte[] bytes = new byte[client.SendBufferSize];
                int recv = 0;
                while (true)
                {
                    recv = stream.Read(bytes, 0, client.SendBufferSize);
                    received += Encoding.UTF8.GetString(bytes, 0, recv);
                    if (received.EndsWith(splitter)) break;
                }

                queue.Enqueue(received);

#else
                //received = await reader.ReadLineAsync();
                received = reader.ReadLine();
#endif
                //text_test.GetComponent<UpdateText>().content = received;


                lastPacket = received;


                string[] str_content = received.Split(splitter);
                Debug.Log(str_content[0]);
                
                try{
                    ConnectChannel connectChannel = JsonConvert.DeserializeObject<ConnectChannel>(str_content[0]);
                    if (connectChannel.identify == "connect"){
                        UnityMainThreadDispatcher._executionQueue.Enqueue(() =>
                        {
                            var clientInfoChannel = new ClientInfoChannel("Oculus Quest Pro", "headset");
                            string message = JsonConvert.SerializeObject(clientInfoChannel);
                            Send_Message(message + splitter);
                            //ChartParking.GetComponent<ChartDisplayer>().InitParkingGraph(str_content);
                        });
                    }
                }

                catch (Exception e){
                    Debug.Log(e);
                }

                try{
                    BlockChannel blockChannel = JsonConvert.DeserializeObject<BlockChannel>(str_content[0]);
                    if (blockChannel.identify == "block"){
                        UnityMainThreadDispatcher._executionQueue.Enqueue(() =>
                        {
                            Debug.Log("Received block data : " + blockChannel.ToString());
                            ExpeBlock expeBlock = ExpeBlock.FromBlockChannel(blockChannel);
                            GameObject.FindFirstObjectByType<ExpeManager>().setCurrentBlock(expeBlock);
                        });
                    }
                }

                catch (Exception e){
                    Debug.Log(e);
                }

                try{
                    UserChannel userChannel = JsonConvert.DeserializeObject<UserChannel>(str_content[0]);
                    if (userChannel.identify == "user"){
                        UnityMainThreadDispatcher._executionQueue.Enqueue(() =>
                        {
                            Debug.Log("Received user data : " + userChannel.ToString());
                            ExpeUser expeUser = ExpeUser.FromUserChannel(userChannel);
                            GameObject.FindFirstObjectByType<ExpeManager>().setCurrentUser(expeUser);
                        });
                    }
                }

                catch (Exception e){
                    Debug.Log(e);
                }


                exchanging = false;
            }
            
            catch (Exception e)
            {
                Debug.Log(e);
            }

            
        }
    }

    public void StopExchange()
    {
        exchangeStopRequested = true;

#if UNITY_EDITOR
                if (exchangeThread != null)
                {
                    connectionThread.Abort();
                    exchangeThread.Abort();
                    stream.Close();
                    client.Close();
                    writer.Close();
                    reader.Close();

                    stream = null;
                    exchangeThread = null;
                }
#else
        if (exchangeTask != null)
        {
            connectionTask.Wait();
            exchangeTask.Wait();
            socket.Dispose();
            writer.Dispose();
            reader.Dispose();

            socket = null;
            exchangeTask = null;
            connectionTask = null;
        }
#endif
        writer = null;
        reader = null;
    }


    public async void Send_Message(String message)
    {
        try
        {
            if (writer != null)
            {
                Debug.Log("Sending : " + message);
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
        }
        catch (Exception e)
        {
            Debug.Log("message failed: " + message + " " + e.ToString());
            //print("Connection Lost, waiting for connection...");
        }

    }

    public void OnDestroy()
    {
        StopExchange();
    }


}
*/