using System.Net.Sockets;

namespace LabirinthClient1;

public class Client
{
    public static void Connection()
    {
        string serverIp = "192.168.56.1";
        int serverPort = 5000;
        List<string[]> matrix = new List<string[]>();
        try
        {
            using TcpClient client = new TcpClient(serverIp, serverPort);
            Console.WriteLine("Connected to server");

            using NetworkStream stream = client.GetStream();
            using StreamWriter streamWriter = new StreamWriter(stream);
            using StreamReader streamReader = new StreamReader(stream);

            streamWriter.AutoFlush = true;
          
            
            int lineCounter = 0;
            while (true)
            {
                
                string line = streamReader.ReadLine();
                if (line == "End")
                {
                    Console.WriteLine("Exiting...");
                    break;
                }
                
                string[] row = line.Split(' ');
                matrix.Add(row);
            }
            Console.WriteLine("The matrix has" + matrix.Count + " elements");
            foreach (var line in matrix)
            {
                Console.WriteLine(string.Join(' ', line));
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}