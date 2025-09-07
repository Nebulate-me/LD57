using _Scripts.Player;
using TMPro;
using UnityEditor;
using UnityEngine;
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
        
        [Header("Player Profile")]
        [SerializeField] private TextMeshProUGUI playerProfileName;
        [SerializeField] private PlayerProfilePopupController playerProfilePopupController;

        [Inject] private IPlayerProfileService _playerProfileService;
        [Inject] private IScreenManager _screenManager;
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
            _screenManager.GoToTutorialScreen();
        }
        
        private void GoToGameScreen()
        {
            _screenManager.GoToGameScreen();
        }
        
        private void GoToHighScoreScreen()
        {
            _screenManager.GoToHighScoreScreen();
        }
        
        private void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#elif UNITY_STANDALONE_WIN
            Application.Quit();
#endif
        }
    }
}