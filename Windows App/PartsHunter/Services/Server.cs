using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SimpleWifi;

namespace PartsHunter.Services
{
    public class Server
    {
        private readonly Wifi wifi;
        private readonly List<AccessPoint> aps;

        private readonly static ManualResetEvent sendDone = new ManualResetEvent(false);

        public Server()
        {


        }

        public static void Send(Socket client, string data)
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
            }
            catch
            {

            }
        }

        public bool Connect_WiFi(AccessPoint ap, string password)
        {
            AuthRequest authRequest = new AuthRequest(ap) {
                Password = password
            };

            return ap.Connect(authRequest);
        }

        public static void TCP_Connect(string host, int port)
        {
            IPAddress[] IPs = Dns.GetHostAddresses(host);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Establishing Connection to {0}", host);
            client.Connect(IPs[0], port);

            Send(client, "This is a test<EOF>");
            Console.WriteLine("Connection established");
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Test()
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Server.Send(client, "This is a test<EOF>");
        }

    }
}
