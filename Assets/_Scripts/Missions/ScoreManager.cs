using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Rooms;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;

namespace _Scripts.Missions
{
    public class ScoreManager : MonoBehaviour, IScoreManager
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [FormerlySerializedAs("restartText")] [SerializeField] private TextMeshProUGUI defeatText;
        [SerializeField] private List<ScoreRank> scoreRanks = new ();

        [Inject] private IDeckManager deckManager;
        [Inject] private IHandManager handManager;
        [Inject] private IMissionManager missionManager;
        
        private int currentScore = 0;

        private void OnEnable()
        {
            SignalsHub.AddListener<MissionCompletedSignal>(OnMissionCompleted);
            SignalsHub.AddListener<DeckUpdatedSignal>(OnDeckUpdated);
            SignalsHub.AddListener<MissionsUpdatedSignal>(OnMissionsUpdated);
        }
        
        private void OnDisable()
        {
            SignalsHub.RemoveListener<MissionCompletedSignal>(OnMissionCompleted);
            SignalsHub.RemoveListener<DeckUpdatedSignal>(OnDeckUpdated);
            SignalsHub.RemoveListener<MissionsUpdatedSignal>(OnMissionsUpdated);
        }

        private void OnMissionCompleted(MissionCompletedSignal signal)
        {
            currentScore += signal.Dto.RewardScore;
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

        private void CheckDefeat()
        {
            if (deckManager.CardAmount <= 0 && handManager.CardAmount <= 0 && missionManager.CompletableMissionsCount <= 0)
            {
                ShowRestartButton();
            }
        }

        private void ShowRestartButton()
        {
            defeatText.text = $"We are out of Resources to build,\n" +
                              $"{GetCurrentRank()}!\n" +
                              $"Final Score: {currentScore}\n" +
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
            if (Input.GetKeyDown(KeyCode.R))
                RestartGame();
        }

        private void UpdateScoreText()
        {
            scoreText.text = $"Score\n" +
                             $"*{currentScore}*\n" +
                             $"{GetCurrentRank()}";
        }

        private string GetCurrentRank()
        {
            var currentRank = scoreRanks.Where(rank => rank.MinScore <= currentScore).OrderByDescending(rank => rank.MinScore).First();
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