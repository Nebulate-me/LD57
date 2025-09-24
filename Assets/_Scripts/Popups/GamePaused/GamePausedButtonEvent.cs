namespace _Scripts.Popups.GamePaused
{
    public readonly struct GamePausedButtonEvent
    {
        public GamePausedPopupButtonType ButtonType { get; }
        public ButtonEventType EventType { get; }
        
        public GamePausedButtonEvent(GamePausedPopupButtonType buttonType, ButtonEventType eventType)
        {
            ButtonType = buttonType;
            EventType = eventType;
        }
    }
}