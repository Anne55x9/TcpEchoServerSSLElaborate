using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    internal class Server
    {
        private int PORT;

        public Server(int port)
        {
            this.PORT = port;
        }

        public void StartServer()
        {
            bool clientCertificateRequired = false;
            bool checkCertificateRevocation = true;
            SslProtocols enabledSSLProtocols = SslProtocols.Tls;

            string serverCertificateFileASW = "C:/CertificatesFinal/ServerSSL.pfx";
            X509Certificate serverCertificate = new X509Certificate2(serverCertificateFileASW, "mysecret");

            TcpListener serverSocket = new TcpListener(IPAddress.Loopback,PORT);
            serverSocket.Start();
            Console.WriteLine("Server started");

            bool leaveInnerSteamOpen = false;

            using (TcpClient connectionSocket = serverSocket.AcceptTcpClient())
            using (Stream unsecureStream = connectionSocket.GetStream())
            using (SslStream sslStream = new SslStream(unsecureStream,leaveInnerSteamOpen))
            {
                sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired,
                    enabledSSLProtocols,checkCertificateRevocation);

                //Forskellig information om SSL udskrevet
                DisplaySecurityLevel(sslStream);
                DisplaySecurityServices(sslStream);
                DisplayCertificateInformation(sslStream);
                DisplayStreamProperties(sslStream);
                DisplayUsage();

                using (StreamReader sr = new StreamReader(sslStream))
                using (StreamWriter sw = new StreamWriter(sslStream))
                {
                    Console.WriteLine("Server activated");
                    sw.AutoFlush = true;

                    string message = sr.ReadLine(); //read string from client
                    string answer = "";
                    while (!string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine("Client: " + message);
                        answer = message.ToUpper();
                        sw.WriteLine(answer);
                        message = sr.ReadLine();
                    }
                }
            }
        }

        void DisplaySecurityLevel(SslStream stream)
        {
            Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
            Console.WriteLine("Hash: {0} strength {1}",stream.HashAlgorithm, stream.HashStrength);
            Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
            Console.WriteLine("Protocol: {0}", stream.SslProtocol);

        }

        void DisplaySecurityServices(SslStream stream)
        {
            Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
            Console.WriteLine("IsSigned: {0}", stream.IsSigned);
            Console.WriteLine("Is encrypted: {0}", stream.IsEncrypted);
        }

        void DisplayCertificateInformation(SslStream stream)
        {
            Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);

            X509Certificate localCertificate = stream.LocalCertificate;
            if (stream.LocalCertificate != null)
            {
                Console.WriteLine("Local cert was issued to {0} an is valid from {1} until {2}.",
                    localCertificate.Subject,
                    localCertificate.GetEffectiveDateString(),
                    localCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Local localCertificate is null.");
            }
            //Display properties of the clients certificate.
            X509Certificate remoteCertificate = stream.RemoteCertificate;
            if (stream.RemoteCertificate != null)
            {
                Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                    remoteCertificate.Subject,
                    remoteCertificate.GetEffectiveDateString(),
                    remoteCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Remote certificate is null.");
            }

        }

        void DisplayStreamProperties(SslStream stream)
        {
            Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
            Console.WriteLine("Can timeout: {0}",stream.IsEncrypted);
        }

        private void DisplayUsage()
        {
            Console.WriteLine("To start the server specify:");
            Console.WriteLine("serverSync certificateFile.cer");
            Environment.Exit(1);
        }

    }
}
