using System;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;
using Newtonsoft.Json;


public class SocketConnection
{
    System.Net.Sockets.TcpClient client;
    System.Net.Sockets.NetworkStream stream;
    private Thread exchangeThread, connectionThread;
    private Byte[] bytes = new Byte[256];
    private StreamWriter writer;
    private BinaryWriter FileWriter;
    private StreamReader reader;
    private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

    public string _host = "192.168.1.44";
    public string _port = "53000";

    const string splitter = "[{//V//}]";

    public SocketConnection()
    {
    }

    public void Init()
    {
        CreateClientThread();
    }

    /*void Start()
    {
        CreateClientThread();
    }*/

    public void CreateClientThread()
    {
        if (connectionThread != null) StopExchange();
        connectionThread = new System.Threading.Thread(Connect);
        connectionThread.Start();
    }

    public void Connect()
    {
        ConnectUnity(_host, _port);
    }


    private void ConnectUnity(string host, string port)
    {
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
    }

    //private bool exchanging = false;
    private bool exchangeStopRequested = false;
    private string lastPacket = null;

    private string errorStatus = null;
    //private string warningStatus = null;
    //private string successStatus = null;
    //private string unknownStatus = null;

    public void StartExchange()
    {
        if (exchangeThread != null) StopExchange();
        exchangeStopRequested = false;
        exchangeThread = new System.Threading.Thread(ReceivePackets);
        exchangeThread.Start();
    }
    private void ReceivePackets()

    {

        while (!exchangeStopRequested)
        {
            if (reader == null) continue;
            //exchanging = true;

            string received = null;

            try
            {
                byte[] bytes = new byte[client.SendBufferSize];
                int recv = 0;
                while (true)
                {
                    recv = stream.Read(bytes, 0, client.SendBufferSize);
                    received += Encoding.UTF8.GetString(bytes, 0, recv);
                    if (received.EndsWith(splitter)) break;
                }

                queue.Enqueue(received);

                //text_test.GetComponent<UpdateText>().content = received;
                /*++msg_per_sec;
                current_calculus_time = GetCurrentUnixTimestampMillis();
                received += " MSG_PER_SEC " + msg_per_sec;
                if (current_calculus_time - start_calculus_time > 1000) // 1 seconde passée
                {
                    start_calculus_time = current_calculus_time;
                    msg_per_sec = 0;
                }
                //received += " TIME " + start_calculus_time + " " + current_calculus_time + " " + (current_calculus_time - start_calculus_time);

                long received_time = GetCurrentUnixTimestampMillis();*/

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
                            GameObject.FindFirstObjectByType<ExpeManager>().currentBlock = expeBlock;
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
                            GameObject.FindFirstObjectByType<ExpeManager>().currentUser = expeUser;
                        });
                    }
                }

                catch (Exception e){
                    Debug.Log(e);
                }

                //exchanging = false;
            }
            
            catch (Exception /*e*/)
            {
                //Debug.Log(e);
            }

            
        }
    }

    public void StopExchange()
    {
        exchangeStopRequested = true;
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
