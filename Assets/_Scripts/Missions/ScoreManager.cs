using _Scripts.Cards;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Missions
{
    public class ScoreManager : MonoBehaviour, IScoreManager
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button restartButton;
        [SerializeField] private int scoreMultiplier = 50;

        [Inject] private IDeckManager deckManager;
        
        private int currentScore = 0;

        private void OnEnable()
        {
            SignalsHub.AddListener<MissionCompletedSignal>(OnMissionCompleted);
        }
        
        private void OnDisable()
        {
            SignalsHub.RemoveListener<MissionCompletedSignal>(OnMissionCompleted);
        }

        private void OnMissionCompleted(MissionCompletedSignal signal)
        {
            currentScore += signal.Dto.RewardScore;
            UpdateScoreText();
        }

        private void Start()
        {
            UpdateScoreText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                RestartGame();
        }

        private void UpdateScoreText()
        {
            scoreText.text = $"Score\n*{currentScore}*";
        }

        public void RestartGame()
        {
            // TODO: Ask in a popup whether Player really wants to restart
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}