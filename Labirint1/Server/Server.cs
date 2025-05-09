using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Labirint1.Server;

public class Server
{
    private static int playerRow;
    private static int playerCol;
    private static string[,] labirinth;
    private static TcpClient[] clients;
    private static StreamReader[] readers;
    private static StreamWriter[] writers;
    private static int numberOfPlayers;

    public Server()
    {
        TcpListener server = null;
        try
        {
            InitializeServer(out server);
            AcceptClients(server);
            GameLoop();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    private void InitializeServer(out TcpListener server)
    {
        Console.WriteLine("Enter number of players (1 or 2): ");
        while (!int.TryParse(Console.ReadLine(), out numberOfPlayers) || (numberOfPlayers != 1 && numberOfPlayers != 2))
        {
            Console.WriteLine("Invalid input. Enter 1 or 2:");
        }

        int port = 5000;
        IPAddress localAddress = IPAddress.Parse("192.168.56.1");
        server = new TcpListener(localAddress, port);
        server.Start();
        Console.WriteLine($"Server started. Waiting for {numberOfPlayers} player(s)...");

        Labirinth lab = new Labirinth();
        labirinth = lab.GetLabirinths();
        DisplayMatrix(labirinth);
        FindPlayer(labirinth);

        clients = new TcpClient[numberOfPlayers];
        readers = new StreamReader[numberOfPlayers];
        writers = new StreamWriter[numberOfPlayers];
    }

    private void AcceptClients(TcpListener server)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            clients[i] = server.AcceptTcpClient();
            NetworkStream stream = clients[i].GetStream();
            readers[i] = new StreamReader(stream);
            writers[i] = new StreamWriter(stream) { AutoFlush = true };
            
            SendMatrix(labirinth, writers[i]);
        }
    }


    private void GameLoop()
    {
        int currentPlayer = 0;
        while (true)
        {
            StreamWriter writer = writers[currentPlayer];
            StreamReader reader = readers[currentPlayer];

            writer.WriteLine("Your turn"); 
            writer.Flush();

            string line = reader.ReadLine();

            if (line == null)
            {
                Console.WriteLine($"Player {currentPlayer + 1} disconnected.");
                break;
            }

            Console.WriteLine($"Player {currentPlayer + 1} command: {line}");

            if (!CheckDirection(line, ref labirinth, writer))
            {
                writer.WriteLine("Invalid input!");
            }

            currentPlayer = (currentPlayer + 1) % numberOfPlayers;
        }
    }


    private static void FindPlayer(string[,] labirinth)
    {
        for (int i = 0; i < labirinth.GetLength(0); i++)
        {
            for (int j = 0; j < labirinth.GetLength(1); j++)
            {
                if (labirinth[i, j] == "1")
                {
                    playerRow = i;
                    playerCol = j;
                }
            }
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
        }

        streamWriter.WriteLine("End");
        streamWriter.Flush();
    }

    private static string[,] HideMatrix(string[,] labirinth)
    {
        int rows = labirinth.GetLength(0);
        int cols = labirinth.GetLength(1);
        string[,] result = new string[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = labirinth[i, j] == "1" ? "1" : "0";
            }
        }

        return result;
    }

    private static void DisplayMatrix(string[,] labirinth)
    {
        for (int i = 0; i < labirinth.GetLength(0); i++)
        {
            Console.WriteLine();
            for (int j = 0; j < labirinth.GetLength(1); j++)
            {
                Console.Write(labirinth[i, j] + " ");
            }
        }

        Console.WriteLine("\n");
    }

    private static bool CheckDirection(string direction, ref string[,] labirinth, StreamWriter writer)
    {
        switch (direction.ToLower())
        {
            case "up": MoveUp(ref labirinth, writer); break;
            case "down": MoveDown(ref labirinth, writer); break;
            case "left": MoveLeft(ref labirinth, writer); break;
            case "right": MoveRight(ref labirinth, writer); break;
            default: return false;
        }

        return true;
    }

    private static void MoveUp(ref string[,] labirinth, StreamWriter writer)
    {
        if (labirinth[playerRow - 1, playerCol] != "0")
        {
            string temp = labirinth[playerRow - 1, playerCol];
            CheckWinningCondition(temp, writer);
            labirinth[playerRow - 1, playerCol] = "1";
            labirinth[playerRow, playerCol] = temp;
            playerRow--;
            SendPosition(writer);
        }
        else SendWallMessage(writer);
    }

    private static void MoveDown(ref string[,] labirinth, StreamWriter writer)
    {
        if (labirinth[playerRow + 1, playerCol] != "0")
        {
            string temp = labirinth[playerRow + 1, playerCol];
            CheckWinningCondition(temp, writer);
            labirinth[playerRow + 1, playerCol] = "1";
            labirinth[playerRow, playerCol] = temp;
            playerRow++;
            SendPosition(writer);
        }
        else SendWallMessage(writer);
    }

    private static void MoveLeft(ref string[,] labirinth, StreamWriter writer)
    {
        if (labirinth[playerRow, playerCol - 1] != "0")
        {
            string temp = labirinth[playerRow, playerCol - 1];
            CheckWinningCondition(temp, writer);
            labirinth[playerRow, playerCol - 1] = "1";
            labirinth[playerRow, playerCol] = temp;
            playerCol--;
            SendPosition(writer);
        }
        else SendWallMessage(writer);
    }

    private static void MoveRight(ref string[,] labirinth, StreamWriter writer)
    {
        if (labirinth[playerRow, playerCol + 1] != "0")
        {
            string temp = labirinth[playerRow, playerCol + 1];
            CheckWinningCondition(temp, writer);
            labirinth[playerRow, playerCol + 1] = "1";
            labirinth[playerRow, playerCol] = temp;
            playerCol++;
            SendPosition(writer);
        }
        else SendWallMessage(writer);
    }

    private static void SendPosition(StreamWriter writer)
    {
        string position = $"{playerRow},{playerCol}";
        writer.WriteLine(position);
        writer.Flush();
        DisplayMatrix(labirinth);
    }

    private static void SendWallMessage(StreamWriter writer)
    {
        writer.WriteLine("You hit a wall!");
        writer.Flush();
        DisplayMatrix(labirinth);
    }

    private static void CheckWinningCondition(string pos, StreamWriter writer)
    {
        if (pos == "3")
        {
            writer.WriteLine("Found the exit! You win!");
            writer.Flush();
        }
    }
}
