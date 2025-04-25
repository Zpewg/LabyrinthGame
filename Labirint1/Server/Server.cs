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
              writer.AutoFlush = true;
                SendMatrix(labirnth, writer);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!checkDirection(line, ref labirnth, writer))
                    {
                        writer.WriteLine("Invalid input!");
                        
                    }

                }

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
            
        }
        streamWriter.WriteLine("End");
        streamWriter.Flush();
        
    }

    public string[,] HideMatrix(string[,] labirnth)
    {
       int rows = labirnth.GetLength(0);
       int cols = labirnth.GetLength(1);
       string[,] result = new string[rows, cols];
        for (int i = 0; i < labirnth.GetLength(0); i++)
        {
            for (int j = 0; j < labirnth.GetLength(1); j++)
            {
                result[i, j] = "1";
                if (labirnth[i, j] != "1")
                {
                    result[i, j] = "0";
                }
                
            }
        }
        return result;
    }
    public static void DisplayMatrix(string[,] labirinth)
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

    public static bool checkDirection(string direction, ref string[,] labirnth, StreamWriter streamWriter)
    {
        switch (direction.ToLower())
        {
            case "up": MoveUp(ref labirnth, streamWriter);
                break; 
            case "down": MoveDown(ref labirnth,streamWriter );
                break;
            case "left": MoveLeft(ref labirnth, streamWriter); 
                break;
            case "right": MoveRight(ref labirnth, streamWriter); 
                break;
            default : return false;
        }
        return true;
    }

    public static void MoveUp(ref string[,] labirnth, StreamWriter streamWriter)
    {
        
        for (int i = 0; i < labirnth.GetLength(0)-1; i++)
        {
            for (int j = 0; j < labirnth.GetLength(1); j++)
            {
                
                if (labirnth[i, j] == "1")
                {
                   
                    if ( labirnth[i - 1, j] != "0")
                    {
                        string temp = labirnth[i - 1, j];
                        CheckWinningCondition(temp, streamWriter);
                        labirnth[i - 1, j] = "1"; 
                        labirnth[i, j] = temp;
                        string position = (i-1).ToString() + "," + j.ToString();
                        Console.WriteLine(position);
                        streamWriter.WriteLine(position);
                        streamWriter.Flush();
                        break;
                    }
                    
                    streamWriter.WriteLine("You hit a wall!");
                    streamWriter.Flush();
                    break;
                }
            }
        }
        DisplayMatrix(labirnth);
    }

    public static void MoveDown(ref string[,] labirnth, StreamWriter streamWriter)
    {
        bool moved = false;
        for (int i = 0; i < labirnth.GetLength(0)-1; i++)
        {
            for (int j = 0; j < labirnth.GetLength(1); j++)
            {
                
                if (labirnth[i, j] == "1")
                {
                   
                    if ( labirnth[i + 1, j] != "0" && !moved)
                    {
                        string temp = labirnth[i + 1, j];
                        CheckWinningCondition(temp, streamWriter);
                        labirnth[i + 1, j] = "1"; 
                        labirnth[i, j] = temp;
                        string position = (i+1).ToString() + "," + j.ToString();
                        Console.WriteLine(position);
                        streamWriter.WriteLine(position);
                        streamWriter.Flush();
                        
                        moved = true;
                        break;
                    }
                    
                    streamWriter.WriteLine("You hit a wall!");
                    streamWriter.Flush();
                    break;
                }
            }
            if(moved) break;
        }
        DisplayMatrix(labirnth);
    }

    public static void MoveLeft(ref string[,] labirnth, StreamWriter streamWriter)
    {
        for (int i = 0; i < labirnth.GetLength(0); i++)
        {
            for (int j = 0; j < labirnth.GetLength(1); j++)
            {
                
                if (labirnth[i, j] == "1")
                {
                   
                    if ( labirnth[i, j-1] != "0")
                    {
                        string temp = labirnth[i, j-1];
                        CheckWinningCondition(temp, streamWriter);
                        labirnth[i, j-1] = "1"; 
                        labirnth[i, j] = temp;
                        string position = i.ToString() + "," + (j-1).ToString();
                        Console.WriteLine(position);
                        streamWriter.WriteLine(position);
                        streamWriter.Flush();
                        break;
                    }

                    
                   streamWriter.WriteLine("You hit a wall!");
                   streamWriter.Flush();
                   break;
                }
            }
        }
        DisplayMatrix(labirnth);
    }

    public static void MoveRight(ref string[,] labirnth, StreamWriter streamWriter)
    {
        for (int i = 0; i < labirnth.GetLength(0); i++)
        {
            for (int j = 0; j < labirnth.GetLength(1); j++)
            {
                
                if (labirnth[i, j] == "1")
                {
                   
                    if (labirnth[i, j+1] != "0")
                    {
                        string temp = labirnth[i, j+1];
                        CheckWinningCondition(temp, streamWriter);
                        labirnth[i, j+1] = "1"; 
                        labirnth[i, j] = temp;
                        string position = i.ToString() + "," + (j+1).ToString();
                        Console.WriteLine(position);
                        streamWriter.WriteLine(position);
                        streamWriter.Flush();
                        break;
                    }
                    
                    streamWriter.WriteLine("You hit a wall!");
                    streamWriter.Flush();
                    break;
                }
            }
        }
        DisplayMatrix(labirnth);
    }

    public static void CheckWinningCondition(string pos, StreamWriter streamWriter)
    {
        if (pos == "3")
        {
            streamWriter.WriteLine("Found the exit! You win!");
            streamWriter.Flush();
            
        }

        
    }
}