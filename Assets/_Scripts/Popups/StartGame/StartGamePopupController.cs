using _Scripts.Missions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Popups.StartGame
{
    public class StartGamePopupController : MonoBehaviour
    {
        [SerializeField] private GameObject popupGameObject;
        [SerializeField] private TMP_InputField playerNameField;
        [SerializeField] private Button startGameButton;

        [Inject] private IScoreManager _scoreManager;

        private void OnEnable()
        {
            playerNameField.onValueChanged.AddListener(OnPlayerNameUpdated);
            startGameButton.onClick.AddListener(StartGame);
        }

        private void OnDisable()
        {
            playerNameField.onValueChanged.RemoveListener(OnPlayerNameUpdated);
            startGameButton.onClick.RemoveListener(StartGame);
        }

        private void OnPlayerNameUpdated(string playerName)
        {
            UpdateStartGameButton(playerName);
        }
        
        private void StartGame()
        {
            _scoreManager.StartGame(playerNameField.text);
            popupGameObject.SetActive(false);
        }

        private void UpdateStartGameButton(string playerName)
        {
            startGameButton.interactable = playerName.Trim() != string.Empty;
        }

        private void Start()
        {
            UpdateStartGameButton(playerNameField.text);
        }
    }
}