using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoClient
{
    internal class Client
    {

        private int PORT;

        public Client(int port)
        {
            this.PORT = port;
        }

        public void StartClient()
        {
            bool leaveInnerStreamOpen = false;

            using (TcpClient connectionSocket = new TcpClient(IPAddress.Loopback.ToString(), PORT))
            using (Stream unsecureStream = connectionSocket.GetStream())
            using (SslStream sslStream = new SslStream(unsecureStream, leaveInnerStreamOpen))
            {
                sslStream.AuthenticateAsClient("FakeServerName");

                using (StreamReader sr = new StreamReader(sslStream))
                using (StreamWriter sw = new StreamWriter(sslStream))
                {
                    Console.WriteLine("Client have connected");
                    sw.AutoFlush = true;

                    Client3(sr,sw);

                    Console.WriteLine("Client finished");
                }

            }     
            
        }

        private void Client3(StreamReader sr, StreamWriter sw)
        {
            for (int i = 0; i < 100; i++)
            {
                string message = "All work and no play makes Jack a dull boy" + i;
                sw.WriteLine(message);
                string serverAnswer = sr.ReadLine();
                Console.WriteLine("Server: " + serverAnswer);
            }
        }

        private void Client2(StreamReader sr, StreamWriter sw)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Type a line");
                string message = Console.ReadLine();
                sw.WriteLine(message);
                string serverAnswer = sr.ReadLine();
                Console.WriteLine("Server: " + serverAnswer);
            }
        }

        private void Client1(StreamReader sr, StreamWriter sw)
        {
            //Sender
            Console.WriteLine("Type a line");
            string message = Console.ReadLine();
            sw.WriteLine(message);

            //Modtager
            string serverAnswer = sr.ReadLine();
            Console.WriteLine("Server" + serverAnswer);

        }

    }
}
