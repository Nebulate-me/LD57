using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Game;
using _Scripts.Game.Timer;
using _Scripts.Missions;
using _Scripts.Missions.Apartment;
using _Scripts.Popups.GameFinished;
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
        [ShowInInspector, ReadOnly] private int _completedMissionsCount = 0;
        [ShowInInspector, ReadOnly] private string _currentPlayerName = string.Empty; 

        public int Score => _currentScore;
        public int CompletedMissionsCount => _completedMissionsCount;
        public string PlayerName => _currentPlayerName;

        public void StartGame(string playerName)
        {
            _currentScore = 0;
            _completedMissionsCount = 0;
            _currentPlayerName = playerName;
            _gameTimerController.StartTimer();
        }

        public void FinishLevel(int emptyRoomTilesCount)
        {
            Debug.Log(
                $"Reducing the current score {_currentScore} by {emptyRoomTilesCount} for every empty or unused tile");
            _currentScore -= emptyRoomTilesCount;
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
            _currentScore += signal.Score;
            _completedMissionsCount++;
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
            SignalsHub.DispatchAsync(new ShowGameFinishedPopupSignal(GameFinishedReason.TimeOut));
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
            var currentRank = GetCurrentRank();
            defeatText.text = $"We are out of Resources to build,\n" +
                              $"{currentRank.RankName}!\n" +
                              $"Final Score: {_currentScore}\n" +
                              $"Press \"R\" to try again.";
            defeatText.gameObject.SetActive(true);
        }

        private void Start()
        {
            defeatText.gameObject.SetActive(false);
            UpdateScoreText();
        }

        private void Update()
        {
            
            // if (Input.GetKeyDown(KeyCode.R))
            //     RestartGame();
            
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

        private void UpdateScoreText()
        {
            scoreText.text = $"Очки\n" +
                             $"*{_currentScore}*\n";
            // $"{GetCurrentRank()}";
        }

        public ScoreRank GetCurrentRank()
        {
            var currentRank = scoreRanks
                .Where(rank => rank is not null && rank.MinCompletedMissions <= _completedMissionsCount)
                .OrderByDescending(rank => rank.MinCompletedMissions).First();
            return currentRank;
        }
        

        public void RestartGame()
        {
            // TODO: Ask in a popup whether Player really wants to restart
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}