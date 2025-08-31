using _Scripts.Missions;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Popups.StartGame
{
    public class StartGamePopupController : BasePopupController
    {
        [SerializeField] private TMP_InputField playerNameField;
        [SerializeField] private Button startGameButton;

        [Inject] private IScoreManager _scoreManager;

        protected override PopupType Type => PopupType.StartGame;

        protected override void SetupSubscriptions()
        {
            base.SetupSubscriptions();
            playerNameField.onValueChanged.AddListener(OnPlayerNameUpdated);
            startGameButton.onClick.AddListener(StartGame);
        }

        protected override void DisposeSubscriptions()
        {
            base.DisposeSubscriptions();
            
            playerNameField.onValueChanged.RemoveListener(OnPlayerNameUpdated);
            startGameButton.onClick.RemoveListener(StartGame);
        }

        private void OnPlayerNameUpdated(string playerName)
        {
            UpdateStartGameButton(playerName);
        }
        
        private void StartGame()
        {
            // _scoreManager.StartGame(playerNameField.text); // TODO: only do this if there is no tutorial
            _scoreManager.PlayerName = playerNameField.text;
            HidePopup();
            SignalsHub.DispatchAsync(new StartGamePopupClosedSignal());
        }

        private void UpdateStartGameButton(string playerName)
        {
            startGameButton.interactable = playerName.Trim() != string.Empty;
        }

        private void Start()
        {
            OnStart();
            UpdateStartGameButton(playerNameField.text);
        }
    }
}