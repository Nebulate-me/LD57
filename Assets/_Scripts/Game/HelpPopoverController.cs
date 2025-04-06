using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Game
{
    public class HelpPopoverController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject helpPopover;
        
        private bool clickedAtLeastOnce;

        private void Start()
        {
            helpPopover.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                clickedAtLeastOnce = true;
                helpPopover.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (clickedAtLeastOnce)
                helpPopover.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (clickedAtLeastOnce)
                helpPopover.SetActive(false);
        }
    }
}
