using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace Echo
{
    class EchoServer
    {
        [Obsolete]
        static void Main(string[] args)
        {

            Console.CancelKeyPress += delegate
            {
                System.Environment.Exit(0);
            };

            TcpListener ServerSocket = new TcpListener(5000);
            ServerSocket.Start();

            Console.WriteLine("Server started.");
            while (true)
            {
                TcpClient clientSocket = ServerSocket.AcceptTcpClient();
                handleClient client = new handleClient();
                client.startClient(clientSocket);
            }


        }
    }

    public class handleClient
    {
        TcpClient clientSocket;
        public void startClient(TcpClient inClientSocket)
        {
            this.clientSocket = inClientSocket;
            Thread ctThread = new Thread(Echo);
            ctThread.Start();
        }



        private void Echo()
        {
            NetworkStream stream = clientSocket.GetStream();
            BinaryReader reader = new BinaryReader(stream);
            BinaryWriter writer = new BinaryWriter(stream);

            while (true)
            {

                string str = reader.ReadString();
                Console.WriteLine(str);

                String response = "";
                String[] splitted = str.Split(" ");

                if (splitted[0] == "GET")
                {
                    if (splitted.Length > 1)
                    {
                        string[] html = System.IO.File.ReadAllLines("../../../www/pub" + splitted[1]);
                        response += "http/1.0 200 OK\n\n";

                        foreach (string line in html)
                        {
                            response += line + "\n";
                        }
                    }
                }
                else
                {
                    response = "Bad request.";
                }

                writer.Write(response);
            }
        }



    }

}