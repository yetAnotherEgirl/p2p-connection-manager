using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace PeerToPeerGamePOC
{
    internal class ConnectionManager
    {
        static int Port;
        string ip = "";

        private TcpClient client;
        private TcpListener listener;

        public bool Connected { get; private set; } = false;

        public ConnectionManager(int port)
        {
            Port = port;
            listener = new TcpListener(IPAddress.Any, port);
            client = new TcpClient();
        }

        public void Connect()
        {
            Console.WriteLine("enter 0 to connect from IP, enter 1 to recieve connection");
            string? input = Console.ReadLine();
            if(input == "0")
            {
                ConnectToIP();
            }
            else if (input == "1")
            {
                RecieveConnection();
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }

        private void ConnectToIP()
        {
            Console.WriteLine("Enter the IP you want to connect to");
            string? input = Console.ReadLine();
            if (input == null)
            {
                Console.WriteLine("Invalid input");
                return;
            }

            ip = input;

            try
            {
                client = new TcpClient(ip, Port);
                Console.WriteLine("Connected to the other player.");
                Connected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to connect to the other player.");
                Console.WriteLine(e.Message);
            }
        }

        private void RecieveConnection()
        {
            int timeoutInSeconds = 10;

            listener.Start();
            Console.WriteLine("Waiting for the other player to connect. . .");
            Stopwatch connectionTimer = Stopwatch.StartNew();

            // times by 1000 to convert seconds into milliseconds
            while (connectionTimer.ElapsedMilliseconds < timeoutInSeconds * 1000)   
            {
                if(listener.Pending())
                {
                    client = listener.AcceptTcpClient();
                    Console.WriteLine("The other player has connected.");
                    Connected = true;
                    break;
                }
                else
                {
                    //sleep for x ammount of milliseconds to prevent high CPU usage
                    Thread.Sleep(1000);
                }
            }

            if (!Connected) Console.WriteLine("failed to recieve incoming connection");
        }

        public void Disconnect()
        {
            client.Close();
            listener.Stop();
            Console.WriteLine("Disconnected from the other player.");
        }

        public void SendGameState(GameState gameState)
        {
            if(!client.Connected)
            {
                Console.WriteLine("Not connected to the other player.");
                return;
            }
            string json = JsonSerializer.Serialize(gameState);
            byte[] data = Encoding.UTF8.GetBytes(json);

            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        public GameState ReceiveGameState()
        {
            if (!client.Connected)
            {
                Console.WriteLine("Not connected to the other player.");
                return new GameState();
            }
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[1024];

            //wait for the data to be sent if not immediatly ready
            while (!stream.DataAvailable) Thread.Sleep(100);

            int bytes = stream.Read(data, 0, data.Length);

            string json = Encoding.UTF8.GetString(data, 0, bytes);
            GameState gameState = JsonSerializer.Deserialize<GameState>(json);

            return gameState;
        }
    }
}
