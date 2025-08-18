using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Game;
using _Scripts.Game.Timer;
using _Scripts.Missions;
using _Scripts.Missions.Apartment;
using Signals;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;

namespace _Scripts.Score
{
    public class ScoreManager : MonoBehaviour, IScoreManager
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [FormerlySerializedAs("restartText")] [SerializeField] private TextMeshProUGUI defeatText;
        [SerializeField] private List<ScoreRank> scoreRanks = new ();

        [Inject] private IDeckManager deckManager;
        [Inject] private IHandManager handManager;
        [Inject] private IMissionManager missionManager;
        [Inject] private ISoundManager soundManager;
        [Inject] private IGameTimerController _gameTimerController;
        [Inject] private IScoreSaver _scoreSaver;
        
        [ShowInInspector, ReadOnly] private int _currentScore = 0;
        [ShowInInspector, ReadOnly] private string _currentPlayerName = string.Empty; 

        public int Score => _currentScore;
        public void StartGame(string playerName)
        {
            _currentScore = 0;
            _currentPlayerName = playerName;
            _gameTimerController.StartTimer();
        }

        private void OnEnable()
        {
            SignalsHub.AddListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
            SignalsHub.AddListener<DeckUpdatedSignal>(OnDeckUpdated);
            SignalsHub.AddListener<MissionsUpdatedSignal>(OnMissionsUpdated);
            _gameTimerController.OnTimerFinished.AddListener(OnTimerFinished);
        }
        
        private void OnDisable()
        {
            SignalsHub.RemoveListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
            SignalsHub.RemoveListener<DeckUpdatedSignal>(OnDeckUpdated);
            SignalsHub.RemoveListener<MissionsUpdatedSignal>(OnMissionsUpdated);
            _gameTimerController.OnTimerFinished.RemoveListener(OnTimerFinished);
        }

        private void OnApartmentMissionCompleted(ApartmentMissionCompletedSignal signal)
        {
            _currentScore += signal.Dto.RewardScore;
            UpdateScoreText();
        }
        
        private void OnDeckUpdated(DeckUpdatedSignal signal)
        {
            CheckDefeat();
        }
        
        private void OnMissionsUpdated(MissionsUpdatedSignal signal)
        {
            CheckDefeat();
        }
        
        private void OnTimerFinished()
        {
            _scoreSaver.SubmitScore(_currentScore, _currentPlayerName);   
            ShowTimeOut();
        }

        private void CheckDefeat()
        {
            // TODO: Improve this check to test whether there is no way to place any of the hand cards on the map
            if (deckManager.RoomCardAmount <= 0 && handManager.CardAmount <= 0 && missionManager.CompletableMissionsCount <= 0)
            {
                soundManager.PlaySound(SoundType.Defeat);
                ShowRestartButton();
            }
        }

        private void ShowRestartButton()
        {
            defeatText.text = $"We are out of Resources to build,\n" +
                              $"{GetCurrentRank()}!\n" +
                              $"Final Score: {_currentScore}\n" +
                              $"Press \"R\" to try again.";
            defeatText.gameObject.SetActive(true);
        }
        
        private void ShowTimeOut()
        {
            defeatText.text = $"Время вышло! Ваш ранг:\n" +
                              $"{GetCurrentRank()}!\n" +
                              $"Очки: {_currentScore}\n" +
                              $"Нажмите \"R\", чтобы начать заново.";
            defeatText.gameObject.SetActive(true);
        }

        private void Start()
        {
            defeatText.gameObject.SetActive(false);
            UpdateScoreText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                RestartGame();
            
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

        private void UpdateScoreText()
        {
            scoreText.text = $"Очки\n" +
                             $"*{_currentScore}*\n";
            // $"{GetCurrentRank()}";
        }

        private string GetCurrentRank()
        {
            var currentRank = scoreRanks.Where(rank => rank.MinScore <= _currentScore).OrderByDescending(rank => rank.MinScore).First();
            return currentRank.Rank;
        }
        

        public void RestartGame()
        {
            // TODO: Ask in a popup whether Player really wants to restart
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}