using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoClient
{
    class Program
    {

        private const int PORT = 7777;
        static void Main(string[] args)
        {
            Client client = new Client(PORT);
            client.StartClient();

            Console.ReadLine();

        }
    }
}
