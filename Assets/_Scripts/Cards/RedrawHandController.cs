using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Cards
{
    public class RedrawHandController : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Button redrawHandButton;

        [Inject] private IHandManager _handManager;


        private void OnEnable()
        {
            redrawHandButton.onClick.AddListener(RedrawHand);
        }

        private void OnDisable()
        {
            redrawHandButton.onClick.RemoveListener(RedrawHand);
        }
        
        private void RedrawHand()
        {
            _handManager.RedrawRoomHand();
            // TODO: Disable until next level start
        }
    }
}