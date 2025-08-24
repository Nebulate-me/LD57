namespace _Scripts.Popups.HighscoreTable
{
    public readonly struct GameFinishedSignal
    {
        public GameFinishedReason Reason { get; }

        public GameFinishedSignal(GameFinishedReason reason)
        {
            Reason = reason;
        }
    }
}