namespace _Scripts.Popups.GameFinished
{
    public readonly struct ShowGameFinishedPopupSignal
    {
        public GameFinishedReason Reason { get; }

        public ShowGameFinishedPopupSignal(GameFinishedReason reason)
        {
            Reason = reason;
        }
    }
}