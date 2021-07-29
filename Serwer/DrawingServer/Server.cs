using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Serwer
{
    class Server
    {
        public int a = 0;
        public int time = 0;
        public bool allowPlayers = true;
        int port;
        TcpListener listener;
        public int players =2;
        public List<TcpClient> clients = new List<TcpClient>();

        static string x = "";

        List<string> icons = new List<string>()
        {
            "!", "!", "N", "N", ",", ",", "k", "k",
            "b", "b", "v", "v", "w", "w", "z", "z"
        };

      static  Random random = new Random();

        public Server(int port = 5432)
        {
            this.port = port;
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
        }

        public void sendToken(int a)
        {
            Console.WriteLine("WYSYLAM TOKEN " + a);
            Points token = new Points();
            token.number =a;
            string czat = "Tura gracza "+(a + 1);
            token.player = czat;
            Envelope tok = new Envelope("Token", token);
            string json = JsonConvert.SerializeObject(tok);
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(json);
            try
            {
                clients[a].GetStream().Write(bytes, 0, bytes.Length);
            }
            catch
                { }

        }

        public void sendToAll(string message)
        {
            double c=0;
            Stopwatch sw = new Stopwatch();
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(message);
            sw.Start();
            for (int x = 0; x < clients.Count; x++)
            {
                try
                {
                    clients[x].GetStream().Write(bytes, 0, bytes.Length);
                }
                catch
                {
                    clients.RemoveAt(x);
                    if (x - 1 < 0)
                    {
                        x--;
                    }
                }
            }
            sw.Stop();
            a++;
            c= sw.Elapsed.TotalMilliseconds * 1000000;
            time += (int)c;

            Console.WriteLine("Minelo  {0} ns ", sw.Elapsed.TotalMilliseconds * 1000000);
            Console.WriteLine(time / a + " Sredni czas ns");
          

        }


      
        public void startGame()
        {

            
            if (clients.Count == players)
            {
                foreach (string s in icons.ToList())
                {
                    int randNumb = random.Next(icons.Count);
                    string t = icons[randNumb];
                    x += t;
                    icons.RemoveAt(randNumb);


                }

                Console.WriteLine(x);
                allowPlayers = false;

                Message piclist = new Message();
                piclist.text = x;

                Envelope ob = new Envelope("Start", piclist);
                string json = JsonConvert.SerializeObject(ob);
                sendToAll(json);
                sendToken(0);

            }
            else
            {
                Console.WriteLine("Czekam na graczy: " + clients.Count);
            }
        }

        public TcpClient newClient()
        {
            TcpClient client = listener.AcceptTcpClient();
            clients.Add(client);
            return client;
        }


        public string recieve(TcpClient client)
        {
            byte[] bytes = new byte[client.ReceiveBufferSize];
            int toRead = client.GetStream().Read(bytes, 0, client.ReceiveBufferSize);
        
            return ASCIIEncoding.ASCII.GetString(bytes, 0, toRead);
        }
    }
}
