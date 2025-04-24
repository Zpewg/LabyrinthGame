using System.Net;
using System.Net.Sockets;
using Labirint1;

namespace Labirint1.Server;


public class Server
{
    public Server()
    {
        TcpListener server = null;

        try
        {
            int port = 5000;
            IPAddress localAdress = IPAddress.Parse("192.168.56.1");
            server = new TcpListener(localAdress, port);
            server.Start();
            Console.WriteLine("Server Started");
            Labirinth lab = new Labirinth();
            string[,] labirnth = lab.GetLabirinths();
            DisplayMatrix(labirnth);
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
              
                SendMatrix(labirnth, writer);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    public void SendMatrix(string[,] labirnth, StreamWriter streamWriter)
    {
        Console.WriteLine("Sending Matrix");
        string[,] hiddenMatrix = HideMatrix(labirnth);
        for (int i = 0; i < hiddenMatrix.GetLength(0); i++)
        {
            string line = "";
            for (int j = 0; j < hiddenMatrix.GetLength(1); j++)
            {
                line += hiddenMatrix[i, j] + " ";
            }
            streamWriter.WriteLine(line);
            Console.WriteLine($"Sent: {line}");
        }
        streamWriter.WriteLine("End");
        streamWriter.Flush();
        
    }

    public string[,] HideMatrix(string[,] labirnth)
    {
        string[,] result = labirnth;
        for (int i = 0; i < labirnth.GetLength(0); i++)
        {
            for (int j = 0; j < labirnth.GetLength(1); j++)
            {
                if (result[i, j] != "1")
                {
                    result[i, j] = "0";
                }
            }
        }
        return result;
    }
    public void DisplayMatrix(string[,] labirinth)
    {
      
        for (int i = 0; i < labirinth.GetLength(0); i++)
        {
            Console.WriteLine(" ");
            for (int j = 0; j < labirinth.GetLength(1); j++)
            {
                Console.Write(labirinth[i, j] + " ");
            }
        }
    }
}