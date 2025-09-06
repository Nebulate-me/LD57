using System;
using _Scripts.Player;
using Signals;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Screens
{
    public class MainMenuScreenController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button playerProfileButton;
        [SerializeField] private Button tutorialScreenButton;
        [SerializeField] private Button gameScreenButton;
        [SerializeField] private Button highScoreScreenButton;
        [SerializeField] private Button exitButton;
        
        [Header("Scene Names")]
        [SerializeField] private string tutorialSceneName = "TutorialScene";
        [SerializeField] private string gameSceneName = "GameScene";
        [SerializeField] private string highScoreSceneName = "HighScoreScene";
        
        [Header("Player Profile")]
        [SerializeField] private TextMeshProUGUI playerProfileName;
        [SerializeField] private PlayerProfilePopupController playerProfilePopupController;

        [Inject] private IPlayerProfileService _playerProfileService;

        private const string ANONYMOUS_PLAYER_NAME = "Аноним";
        private void OnEnable()
        {
            playerProfileButton.onClick.AddListener(ShowPlayerProfilePopup);
            tutorialScreenButton.onClick.AddListener(GoToTutorialScreen);
            gameScreenButton.onClick.AddListener(GoToGameScreen);
            highScoreScreenButton.onClick.AddListener(GoToHighScoreScreen);
            exitButton.onClick.AddListener(ExitGame);
            
            SignalsHub.AddListener<PlayerProfilePopupClosedSignal>(OnPlayerProfilePopupClosed);
        }

        private void OnDisable()
        {
            playerProfileButton.onClick.RemoveListener(ShowPlayerProfilePopup);
            tutorialScreenButton.onClick.RemoveListener(GoToTutorialScreen);
            gameScreenButton.onClick.RemoveListener(GoToGameScreen);
            highScoreScreenButton.onClick.RemoveListener(GoToHighScoreScreen);
            exitButton.onClick.RemoveListener(ExitGame);
            
            SignalsHub.RemoveListener<PlayerProfilePopupClosedSignal>(OnPlayerProfilePopupClosed);
        }

        private void Start()
        {
            if (_playerProfileService.TryGetPlayerByName(ANONYMOUS_PLAYER_NAME, out var existingAnonymousPlayer))
            {
                _playerProfileService.TrySetCurrentPlayerById(existingAnonymousPlayer.Id, out _);
            }
            else
            {
                _playerProfileService.TryCreatePlayer(ANONYMOUS_PLAYER_NAME, out var newAnonymousPlayer);
                _playerProfileService.TrySetCurrentPlayerById(newAnonymousPlayer.Id, out _);
            }
            UpdatePlayerProfileName();
        }

        private void ShowPlayerProfilePopup()
        {
            playerProfilePopupController.Open();
        }

        private void GoToTutorialScreen()
        {
            GoToScreen(tutorialSceneName);
        }
        
        private void GoToGameScreen()
        {
            GoToScreen(gameSceneName);
        }
        
        private void GoToHighScoreScreen()
        {
            GoToScreen(highScoreSceneName);
        }
        
        private void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#elif UNITY_STANDALONE_WIN
            Application.Quit();
#endif
        }
        
        private void OnPlayerProfilePopupClosed(PlayerProfilePopupClosedSignal signal)
        {
            UpdatePlayerProfileName();
        }

        private void UpdatePlayerProfileName()
        {
            playerProfileName.text = _playerProfileService.CurrentPlayer.Name;
        }

        private void GoToScreen(string screenName)
        {
            SceneManager.LoadScene(screenName);
        }
    }
}