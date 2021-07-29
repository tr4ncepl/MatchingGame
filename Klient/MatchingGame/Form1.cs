using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MatchingGame
{
    public partial class Form1 : Form
    {
        bool allowClick = false;

        int points = 0;
        Label firstClicked = null;

        //static string fc = "";
        //static string sec = "";

        public int a = 0;
        static System.Net.Sockets.TcpClient client;
        static NetworkStream stream;


        Label secondClicked = null;

        public int Abc { get; set; }

        static void wait(double x)
        {
            DateTime t = DateTime.Now;
            DateTime tf = DateTime.Now.AddSeconds(x);

            while (t < tf)
            {
                t = DateTime.Now;
            }
        }

        private void TestShow(string number,string number2)
        {
            foreach (Control control1 in tableLayoutPanel1.Controls)
            {
                Label klabel1 = control1 as Label;
                if (klabel1.Name == number)
                {
                    klabel1.ForeColor = Color.Black;
                    wait(0.75);
                    TestHide(number, number2);
                    
                }
            }
        }


        private void Show(string number)
        {
            foreach(Control control3 in tableLayoutPanel1.Controls)
            {
                Label klabel3 = control3 as Label;
                {
                    if (klabel3.Name==number)
                    {
                        klabel3.ForeColor = Color.Black;
                    }
                }
            }
        }
        private void TestHide(string numb, string numb2)
        {

            foreach (Control control2 in tableLayoutPanel1.Controls)
            {
                Label klabel2 = control2 as Label;
                if (klabel2.Name == numb|| klabel2.Name==numb2)
                {
                    klabel2.ForeColor = klabel2.BackColor;
                    
                }
            }
        }

        private void AssignIconsToSquares(string ciag2)
        {
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;
                if (iconLabel != null)
                {
                    iconLabel.Text = ciag2.Substring(a, 1);
                    iconLabel.ForeColor = iconLabel.BackColor;
                }
                a++;
            }
        }

        private void SendBack(int id)
        {
            Points back = new Points();
            back.number = id;
            Envelope tback = new Envelope("TokenB", back);
            
            string pkt = JsonConvert.SerializeObject(tback);
            byte[] messageToSend = ASCIIEncoding.ASCII.GetBytes(pkt);
            stream.Write(messageToSend, 0, messageToSend.Length);

        }
        

        private void TimerRemaining_Tick(object sender, EventArgs e)
        {
           
            int timer = Convert.ToInt32(timeLeft.Text);
            timer -= 1;
            timeLeft.Text = Convert.ToString(timer);
            if (timer ==0)
            {
                TimeRemaining.Stop();
                TimeRemaining.Enabled = false;
                MessageBox.Show("End of time");
                
            }
        }

        private void Form1_Load()
        {
            timeLeft.Text = "10";
            TimeRemaining.Enabled = true;
            TimeRemaining.Tick += new EventHandler(TimerRemaining_Tick);
            
        }

        
        
       



        public Form1()
        {
            do
            {
                try
                {
                    Form2 ipdia = new Form2();
                    ipdia.ShowDialog();
                    if (ipdia.close == true)
                    {
                        System.Environment.Exit(0);
                    }
                    client = new TcpClient(ipdia.ip, 5432);
                    stream = client.GetStream();
                    break;
                }
                catch
                {

                }

            }
            while (true);

            Task.Run(() =>
            {
                while (true)
                {
                    
                    byte[] messagerecieve = new byte[client.ReceiveBufferSize];
                    int read = stream.Read(messagerecieve, 0, client.ReceiveBufferSize);
                    string obj = Encoding.ASCII.GetString(messagerecieve, 0, read);

                    try
                    {
                        while (true)
                        {
                            int index = obj.IndexOf("}}") + 1;
                            string objtemp = obj.Substring(0, index + 1);
                            Envelope unknown = JsonConvert.DeserializeObject<Envelope>(objtemp);
                           



                            switch (unknown.type)
                            {
                                case "Znak":
                                    Picture pp = JsonConvert.DeserializeObject<Picture>(unknown.obj.ToString());
                                    string tt = pp.znak;
                                  
                                    ChangeColor(tt);
                                    break;

                                case "Message":
                                    Message m = JsonConvert.DeserializeObject<Message>(unknown.obj.ToString());
                                    string s = m.user + ": " + m.text + " \r\n";
                                    chatWindow.Text += s;
                                    break;

                                case "Start":
                                    Message obrazki = JsonConvert.DeserializeObject<Message>(unknown.obj.ToString());
                                    string obr = obrazki.text;
                                    AssignIconsToSquares(obr);

                                    chatWindow.Text += "Serwer: Start rozgrywki \r\n";
                                   

                                    break;

                                case "Koniec":
                                    Message k = JsonConvert.DeserializeObject<Message>(unknown.obj.ToString());
                                    string q = k.text;
                                   
                                    Points punkty = new Points();
                                    punkty.number = points;
                                    punkty.player = userName.Text;
                                  
                           
                                    Envelope en = new Envelope("Punkty", punkty);
                                    string pkt = JsonConvert.SerializeObject(en);
                                    byte[] messageToSend = ASCIIEncoding.ASCII.GetBytes(pkt);
                                    stream.Write(messageToSend, 0, messageToSend.Length);
                                    
                                    break;

                                case "Tabela":
                                    Points t = JsonConvert.DeserializeObject<Points>(unknown.obj.ToString());
                                    string us = t.player;
                                  int  po = t.number;
                                    string final = "Wygral gracz  " + us + " z liczba punktow "+ po +"  Gratulacje !!!" + "\n Liczba twoich puntkow to " + points;
                                    MessageBox.Show(final);
                                    Close();
                                    break;

                                case "First":
                                    Message f = JsonConvert.DeserializeObject<Message>(unknown.obj.ToString());
                                    string fc = f.text;
                                    Show(fc);
                                    break;

                                case "Second":
                                    Message sc = JsonConvert.DeserializeObject<Message>(unknown.obj.ToString());
                                    string sec11 = sc.text;
                                    string sec22 = sc.user;
                                    TestShow(sec11, sec22);
                                    break;

                                case "Token":
                                    Points tok = JsonConvert.DeserializeObject<Points>(unknown.obj.ToString());
                                    Abc = tok.number;
                                    string czat = tok.player;
                                    chatWindow.Text += czat+"\r\n";
                                    timeLeft.Text = "10";
                                    allowClick = true;
                                    break; 
                            }
                           if (objtemp.Length == obj.Length)
                            {
                                break;
                            }
                            obj = obj.Substring(index + 1, obj.Length - index - 1);
                        }
                    }
                    catch
                    {

                    }

                }
            });
            InitializeComponent();
        }

        private void label_Click(object sender, EventArgs e)
        {
          

            message.Text = "Punkty : " + points;
            if (!allowClick) return;


            if (timer1.Enabled == true)
                return;



            Label clickedLabel = sender as Label;
         
            if (clickedLabel != null)
            {

                if (clickedLabel.ForeColor != clickedLabel.BackColor)
                {
                    MessageBox.Show("Nie mozesz tego odkryć");
                    return;

                }

                if (firstClicked == null)
                {
                    //MessageBox.Show(clickedLabel.Name);
                    firstClicked = clickedLabel;
                    firstClicked.ForeColor = Color.Black;
                   
                    Message fc = new Message();
                    fc.text = firstClicked.Name;
                    Envelope env = new Envelope("First", fc);
                   string o = JsonConvert.SerializeObject(env);
                   byte[] msgToSend = ASCIIEncoding.ASCII.GetBytes(o);
                    stream.Write(msgToSend, 0, msgToSend.Length);

                    return;
                }

                
                    secondClicked = clickedLabel;
                secondClicked.ForeColor = Color.Black;
                TimeRemaining.Stop();
               // timer.Stop();
                CheckForWinner();
                allowClick = false;
               SendBack(Abc);


                if (firstClicked.Text == secondClicked.Text)
                {

                    points += 10;
                    message.Text = "Punkty : " + points;
                    Picture p = new Picture();
                    p.znak = firstClicked.Text;
                    Envelope en = new Envelope("Znak", p);
                    string obj = JsonConvert.SerializeObject(en);
                    byte[] messageToSend = ASCIIEncoding.ASCII.GetBytes(obj);
                    stream.Write(messageToSend, 0, messageToSend.Length);
                    firstClicked = null;
                    secondClicked = null;
                    
                    return;
                }
                else
                {
                    points += -1;
                    message.Text = "Punkty : " + points;
                     timer1.Start();

                    Message sc = new Message();
                    sc.text = secondClicked.Name;
                    sc.user = firstClicked.Name;
                    Envelope env = new Envelope("Second", sc);
                    string o = JsonConvert.SerializeObject(env);
                    byte[] msgToSend = ASCIIEncoding.ASCII.GetBytes(o);
                    stream.Write(msgToSend, 0, msgToSend.Length);


                }        
            }

        }


        private void ChangeColor(string kolor)
        {
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label klabel = control as Label;
                if (klabel != null)
                {
                    if (klabel.Text == kolor)
                    {
                        klabel.ForeColor = Color.Pink;
                    }
                }
            }
        }




        private void timer1_Tick(object sender, EventArgs e)
        {

            timer1.Stop();
          

            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;


            firstClicked = null;
            secondClicked = null;
            
          
        }


        private void CheckForWinner()
        {

            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;

                if (iconLabel != null)
                {
                    if (iconLabel.ForeColor == iconLabel.BackColor)
                    {
                        return;
                    }
                }
            }
            
            Points winner = new Points();
            winner.player = userName.Text;
            winner.number = points+10;
            Envelope env = new Envelope("Koniec", winner);
            string o = JsonConvert.SerializeObject(env);
            byte[] msgToSend = ASCIIEncoding.ASCII.GetBytes(o);
            stream.Write(msgToSend, 0, msgToSend.Length);
           
        }

        private void send_Click(object sender, EventArgs e)
        {
            if (userName.Text.Length > 3 && wiadomosc.Text != "")
            {
                Message m = new Message();
                m.text = wiadomosc.Text;
                m.user = userName.Text;
                message.Text = "";
                Envelope en = new Envelope("Message", m);
                string obj = JsonConvert.SerializeObject(en);
                byte[] messageToSend = ASCIIEncoding.ASCII.GetBytes(obj);
                stream.Write(messageToSend, 0, messageToSend.Length);

            }
            else
            {
                MessageBox.Show("Nick musi zawierac conajmniej 3 znaki oraz wiadomosc nie moze byc pusta !!!");
            }
        }

        
    }
}
