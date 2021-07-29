namespace MatchingGame
{
    class Envelope
    {
        public string type;
        public object obj;

        public Envelope(string type, object obj)
        {
            this.obj = obj;
            this.type = type;
        }
    }
}
