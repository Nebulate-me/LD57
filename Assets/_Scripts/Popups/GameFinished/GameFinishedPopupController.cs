using System;
using _Scripts.Missions;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Popups.GameFinished
{
    public class GameFinishedPopupController : BasePopupController
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI gameFinishedReason;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI rankNameText;
        [SerializeField] private TextMeshProUGUI rankDescriptionText;
        
        [Header("Buttons")]
        [SerializeField] private Button highScoreButton;
        [SerializeField] private Button restartGameButton;

        [Inject] private IScoreManager _scoreManager;

        protected override PopupType Type => PopupType.GameFinished;

        private void Start()
        {
            OnStart();
        }

        protected override void SetupSubscriptions()
        {
            base.SetupSubscriptions();
            SignalsHub.AddListener<ShowGameFinishedPopupSignal>(OnShowGameFinishedPopup);
            
            highScoreButton.onClick.AddListener(OnHighScoreButtonClicked);
            restartGameButton.onClick.AddListener(OnRestartGameButtonClicked); 
        }
        
        protected override void DisposeSubscriptions()
        {
            base.DisposeSubscriptions();
            
            SignalsHub.RemoveListener<ShowGameFinishedPopupSignal>(OnShowGameFinishedPopup);
            highScoreButton.onClick.RemoveListener(OnHighScoreButtonClicked);
            restartGameButton.onClick.RemoveListener(OnRestartGameButtonClicked);
        }
        
        private void OnShowGameFinishedPopup(ShowGameFinishedPopupSignal signal)
        {
            gameFinishedReason.text = GetGameFinishedText(signal.Reason);
            playerNameText.text = _scoreManager.PlayerName;
            scoreText.text = _scoreManager.Score.ToString();
            var rank = _scoreManager.GetCurrentRank();
            rankNameText.text = rank.RankName;
            rankDescriptionText.text = rank.RankDescription;
            
            ShowPopup();
        }

        private string GetGameFinishedText(GameFinishedReason reason)
        {
            return reason switch
            {
                GameFinishedReason.TimeOut => "Время вышло!",
                GameFinishedReason.AllLevelsCompleted => "Игра пройдена!",
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
            };
        }

        private void OnHighScoreButtonClicked()
        {
            HidePopup();
            SignalsHub.DispatchAsync(new ShowPopupSignal(PopupType.HighScore));
        }
        
        private void OnRestartGameButtonClicked()
        {
            HidePopup();
            
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}