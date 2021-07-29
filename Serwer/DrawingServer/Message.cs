namespace Serwer
{
    class Message
    {
        public string text { get; set; }
        public string user { get; set; }


        public override string ToString()
        {
            return string.Format("{0}", user);
        }

        
    }
}
