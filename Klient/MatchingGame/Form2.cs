using System;
using System.Windows.Forms;

namespace MatchingGame
{
    public partial class Form2 : Form
    {
        public string ip;
        public bool close;

        public Form2()
        {
            InitializeComponent();
            close = true;
        }

        private void Login_Click(object sender, EventArgs e)
        {
            close = false;
            ip = textBox1.Text;
            this.Close();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
