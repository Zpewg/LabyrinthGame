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
            DisplayMatrix();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    public void DisplayMatrix()
    {
        Labirinth lab = new Labirinth();
        string[,] labirnth = lab.GetLabirinths();
        for (int i = 0; i < labirnth.GetLength(0); i++)
        {
            Console.WriteLine(" ");
            for (int j = 0; j < labirnth.GetLength(1); j++)
            {
                Console.Write(labirnth[i, j] + " ");
            }
        }
    }
}