using Signals;
using UnityEngine;

namespace _Scripts.Popups
{
    public abstract class BasePopupController : MonoBehaviour
    {
        [SerializeField] private GameObject popup;
        [SerializeField] private bool showOnStartUp = false;
        
        protected bool IsShown = false;

        protected abstract PopupType Type { get; }

        protected void OnStart()
        {
            IsShown = popup.activeSelf;
            if (showOnStartUp)
            {
                ShowPopup();
            }
            else
            {
                HidePopup();
            }
        }

        private void OnEnable()
        {
            SetupSubscriptions();
        }

        private void OnDisable()
        {
            DisposeSubscriptions();
        }

        protected virtual void SetupSubscriptions()
        {
            SignalsHub.AddListener<ShowPopupSignal>(OnShowPopupSignal);
            SignalsHub.AddListener<HidePopupSignal>(OnHidePopupSignal);
        }

        protected virtual void DisposeSubscriptions()
        {
            SignalsHub.RemoveListener<ShowPopupSignal>(OnShowPopupSignal);
            SignalsHub.RemoveListener<HidePopupSignal>(OnHidePopupSignal);
        }

        private void OnShowPopupSignal(ShowPopupSignal signal)
        {
            if (signal.PopupType == Type)
            {
                ShowPopup();
            }
            else
            {
                HidePopup();
            }
        }
        
        private void OnHidePopupSignal(HidePopupSignal signal)
        {
            if (signal.PopupType == Type)
            {
                HidePopup();
            }
        }

        protected void ShowPopup()
        {
            if (IsShown) return;
            IsShown = true;
            popup.SetActive(true);
            // TODO: Animation
            OnShowPopup();
        }

        protected virtual void OnShowPopup()
        {
        }

        protected void HidePopup()
        {
            if (!IsShown) return;
            IsShown = false;
            
            popup.SetActive(false);
            
            OnHidePopup();
        }
        
        protected virtual void OnHidePopup()
        {
        }
    }
}