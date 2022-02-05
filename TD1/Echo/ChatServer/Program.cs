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

                char char_read = ' ';
                char[] last_chars_buffer = new char[4];
                char[] request_end = { (char)0x0A, (char)0x0D, (char)0x0A, (char)0x0D };
                string request = "";
                bool must_continue = false;

                do
                {
                    try
                    {
                        char_read = reader.ReadChar();
                        request += char_read;
                    }
                    catch (EndOfStreamException e)
                    {
                        must_continue = true;
                        break;
                    }

                    for (int i = 3; i > 0; i--)
                    {
                        last_chars_buffer[i] = last_chars_buffer[i - 1];
                    }
                    last_chars_buffer[0] = char_read;
                }
                while (!last_chars_buffer.SequenceEqual(request_end));

                if (must_continue)
                {
                    continue;
                }

                Console.WriteLine(request);

                String response = "";
                String[] splitted = request.Split(" ");

                if (splitted[0] == "GET")
                {
                    if (splitted.Length > 1)
                    {
                        string[] html = { };

                        try
                        {
                            html = System.IO.File.ReadAllLines("../../../www/pub" + splitted[1]);
                        }
                        catch (FileNotFoundException e)
                        {
                            
                        }
                        response += "http/1.0 200 OK\n";
                        response += "Content-Type: text/html\n\n";

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