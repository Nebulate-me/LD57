namespace _Scripts.Popups
{
    public readonly struct ShowPopupSignal
    { 
        public PopupType PopupType { get; }

        public ShowPopupSignal(PopupType popupType)
        {
            PopupType = popupType;
        }
    }
}