namespace PeerToPeerGamePOC
{
    public struct GameState
    {
        public string LastMove { get; set; }

        public GameState(string lastMove) : this()
        {
            LastMove = lastMove;
        }
    }
}