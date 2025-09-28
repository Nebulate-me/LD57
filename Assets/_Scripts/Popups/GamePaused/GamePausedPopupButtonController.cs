using Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.Popups.GamePaused
{
    public class GamePausedPopupButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private GamePausedPopupButtonType buttonType = GamePausedPopupButtonType.None;

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }
        
        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            SignalsHub.DispatchAsync(new GamePausedButtonEvent(buttonType, ButtonEventType.PointerClick));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SignalsHub.DispatchAsync(new GamePausedButtonEvent(buttonType, ButtonEventType.PointerEnter));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SignalsHub.DispatchAsync(new GamePausedButtonEvent(buttonType, ButtonEventType.PointerExit));
        }
    }
}