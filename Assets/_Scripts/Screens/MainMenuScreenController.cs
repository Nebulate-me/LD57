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
        private void OnEnable()
        {
            tutorialScreenButton.onClick.AddListener(GoToTutorialScreen);
            gameScreenButton.onClick.AddListener(GoToGameScreen);
            highScoreScreenButton.onClick.AddListener(GoToHighScoreScreen);
            exitButton.onClick.AddListener(ExitGame);
        }

        private void OnDisable()
        {
            tutorialScreenButton.onClick.RemoveListener(GoToTutorialScreen);
            gameScreenButton.onClick.RemoveListener(GoToGameScreen);
            highScoreScreenButton.onClick.RemoveListener(GoToHighScoreScreen);
            exitButton.onClick.RemoveListener(ExitGame);
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

        private void GoToScreen(string screenName)
        {
            SceneManager.LoadScene(screenName);
        }
    }
}