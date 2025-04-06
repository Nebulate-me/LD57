using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Missions
{
    public class ScoreManager : MonoBehaviour, IScoreManager
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        
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

        private void UpdateScoreText()
        {
            scoreText.text = $"Score\n*{currentScore}*";
        }

        public void RestartGame()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}