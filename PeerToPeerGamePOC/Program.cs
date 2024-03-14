namespace PeerToPeerGamePOC
{
    internal class Program
    {
        const int port = 12345;
        static readonly ConnectionManager ConnectionManager = new(port);
        static void Main(string[] args)
        {
            ConnectionManager.Connect();

            if (!ConnectionManager.Connected)
            {
                return;
            }

            bool exiting = false;

            while (!exiting)
            {
                Console.WriteLine("\nEnter 0 to send dummy data, enter 1 to recieve dummy data, enter Q to exit");
                string? input = Console.ReadLine();

                switch (input)
                {
                    case null:
                        break;
                    case "0":
                        SendDummyData();
                        break;
                    case "1":
                        RecieveDummyData();
                        break;
                    case "q":
                    case "Q":
                        exiting = true;
                        ConnectionManager.Disconnect();
                        break;
                }
            }

        }

        static void SendDummyData()
        {
            if (!ConnectionManager.Connected)
            {
                Console.WriteLine("no connection established");
                return;
            }

            Console.WriteLine("enter dummy string to send");
            string? input = Console.ReadLine();
            if (input == null)
            {
                Console.WriteLine("no string entered");
                return;
            }
            GameState exampleGameState = new(input);

            ConnectionManager.SendGameState(exampleGameState);
        }

        static void RecieveDummyData()
        {
            if (!ConnectionManager.Connected)
            {
                Console.WriteLine("no connection established");
                return;
            }

            Console.WriteLine("waiting for data. . .");
            GameState dummyGameState = ConnectionManager.ReceiveGameState();
            Console.WriteLine($"recieved: {dummyGameState.LastMove}");
        }
    }
}
