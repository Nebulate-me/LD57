using System;
using _Scripts.Screens;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Player
{
    public class PlayerProfileGroupController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerProfileName;
        [SerializeField] private Button playerNamePopupButton;
        [SerializeField] private PlayerProfilePopupController playerProfilePopupController;

        [Inject] private IPlayerProfileService _playerProfileService;
        
        private void OnEnable()
        {
            playerNamePopupButton.onClick.AddListener(OpenPlayerNamePopup);
            
            SignalsHub.AddListener<PlayerProfilePopupClosedSignal>(OnPlayerProfilePopupClosed);
        }

        private void OnDisable()
        {
            playerNamePopupButton.onClick.RemoveListener(OpenPlayerNamePopup);
            
            SignalsHub.RemoveListener<PlayerProfilePopupClosedSignal>(OnPlayerProfilePopupClosed);
        }

        private void OpenPlayerNamePopup()
        {
            playerProfilePopupController.Open();
        }
        
        private void OnPlayerProfilePopupClosed(PlayerProfilePopupClosedSignal signal)
        {
            UpdatePlayerProfileName();
        }

        private void Start()
        {
            UpdatePlayerProfileName();
        }

        private void UpdatePlayerProfileName()
        {
            playerProfileName.text = _playerProfileService.CurrentPlayer.Name;
        }
    }
}