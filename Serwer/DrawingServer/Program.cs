using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serwer
{
    class Program
    {

        public static List<Points> points = new List<Points>();
        static Server server = new Server(5432);

        static void Main(string[] args)
        {


            while (true)
            {


                // add client to server

                TcpClient client = server.newClient();

                // connect new client in seperate thread
                Task.Run(() =>
                {
                    //server.Assign();
                    server.startGame();
                   
                    Console.WriteLine("user joined");
                   // server.clients.ForEach(Console.WriteLine);

                    // while thread is running, send message, else exit loop.
                    while (true)
                    {
                        try
                        {

                            string message = server.recieve(client);
                            int index = message.IndexOf("}}") + 1;
                            string objtemp = message.Substring(0, index + 1);
                            
                            Envelope unknown = JsonConvert.DeserializeObject<Envelope>(message);
                            
                   
                            
                            if (unknown.type == "Punkty")
                            {
                                Points m = JsonConvert.DeserializeObject<Points>(unknown.obj.ToString());
                                points.Add(m);


                                if (server.players == points.Count)
                                {
                                 
                                    foreach (Points item in points.OrderBy(x => x.number))
                                        Console.WriteLine(item);
                                  int maxNumb = points.Max(t => t.number);

                                    string player = points.Find(i => i.number == maxNumb).ToString();


                                    Points end = new Points();
                                    end.number = maxNumb;
                                    end.player = player;

                                    Console.WriteLine(end.number + end.player);

                                    Envelope en = new Envelope("Tabela", end);
                                    string json = JsonConvert.SerializeObject(en);

                                    server.sendToAll(json);
                                    points.Clear();
                                    Console.WriteLine("Wyczyszczono liste punktow");
                                    server.clients.Clear();
                                    Console.WriteLine("Wyczyszczono liste klientow");
                                    
                                }


                            }
                            else
                                if(unknown.type=="TokenB")
                            {
                              
                                Points tk = JsonConvert.DeserializeObject<Points>(unknown.obj.ToString());
                                int a = tk.number;
                                Console.WriteLine(a + "OTRZYMALEM");

                                if (a==server.players-1)
                                {
                                    a = 0;
                                    server.sendToken(a);
                                }
                                else
                                {
                                    a = a + 1;
                                    server.sendToken(a);
                                }
                              
                            }
                            else
                            {
                                if(unknown.type=="Koniec")
                                {
                                    Points q = JsonConvert.DeserializeObject<Points>(unknown.obj.ToString());
                                    points.Add(q);
                                }
                            }
                            Console.WriteLine("recieved " + message);
                            

                            server.sendToAll(message);


                        }
                        catch
                        {
                            break;
                        }
                    }
                });
            }
            // wait for client
        }
    }
}
