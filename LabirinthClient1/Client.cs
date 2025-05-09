using System.Data;
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
            PrintRules();
            bool running = true;
            while (running)
            {
                string line = Console.ReadLine();
                switch (line)
                {
                    case "1": Console.WriteLine("Good luck!");
                        running = false;
                        break;
                    case "0": Console.WriteLine("Good bye!");
                        return;
                    default: Console.WriteLine("Not a possible funciton"); break;
                }
            }
            while (true)
            {
                
                string line = streamReader.ReadLine();
                if (line == "End")
                {
                    break;
                }
                
                string[] row = line.Split(' ');
                matrix.Add(row);
            }
            
           string[,] matrix2D = ListToMatrix(matrix);
           PrintMatrix(matrix2D);

           string messageFromServer;
           while (true)
           {
               string turnMessage = streamReader.ReadLine();
               if (turnMessage != "Your turn")
               {
                   Console.WriteLine("Error, another message was sent!");
                   continue;
               }
               Console.WriteLine(turnMessage);
               Console.WriteLine(turnMessage);
               Console.WriteLine("\nWhat is your move?");
               string message = Console.ReadLine();
               streamWriter.WriteLine(message);
               
               messageFromServer = streamReader.ReadLine();
               if (messageFromServer == "Found the exit! You win!")
               {
                   Console.WriteLine("Congratulations! You win!");
                   break;
               }
               if (messageFromServer == "You hit a wall!")
               {
                   Console.WriteLine("You hit a wall!");
                   continue;
               }

               if (messageFromServer == "Invalid input!")
               {
                   Console.WriteLine("Invalid input!");
                   continue;
               }
               string[] coordinates = messageFromServer.Split(',');
               int newRow = int.Parse(coordinates[0]);
               int newCol = int.Parse(coordinates[1]);
           
               Console.WriteLine(newRow + "," + newCol);
               MatrixUpdatedValues(ref matrix2D, newRow, newCol);
               
               
           }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public static void PrintRules()
    {
        Console.WriteLine("Hello and Welcome to the Labyrinth Game!"
                          +"\n The rules are simple 1 represents your postion and you have to find the exit"
                          +"\n Type 'up' to go up"
                          +"\n Type 'down' to go down"
                          +"\n Type 'left' to go left"
                          +"\n Type 'right' to go right"
                          +"\n Type 1 if you wish to continue ore 0 to leave");
    }
    public static void PrintMatrix(string[,] matrix2D)
    {
        for (int i = 0; i < matrix2D.GetLength(0); i++)
        {
            Console.WriteLine(" ");
            for (int j = 0; j < matrix2D.GetLength(1); j++)
            {
                Console.Write(matrix2D[i, j] + " ");
            }
        }
    }

    public static void MatrixUpdatedValues(ref string[,] matrix, int row, int collumn)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (matrix[i, j] == "1")
                {
                    matrix[i, j] = "0";
                    matrix[row, collumn] = "1";
               
                }
            }
        }
        PrintMatrix(matrix);
     
    }

    public static string[,] ListToMatrix(List<string[]> matrix)
    {
        int rows = matrix.Count;
        int cols = matrix[0].Length;
        string[,] matrix2D = new string[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix2D[i, j] = matrix[i][j];
            }
        }
        return matrix2D;
    }
}