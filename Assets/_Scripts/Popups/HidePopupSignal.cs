namespace _Scripts.Popups
{
    public readonly struct HidePopupSignal
    { 
        public PopupType PopupType { get; }

        private HidePopupSignal(PopupType popupType)
        {
            PopupType = popupType;
        }
    }
}