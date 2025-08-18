using System;
using _Scripts.Score;
using Signals;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Popups.HighscoreTable
{
    public class HighScorePopupController : MonoBehaviour
    {
        [SerializeField] private GameObject popup;
        [SerializeField] private Transform contentParent;
        [FormerlySerializedAs("highscoreEntryRowPrefab")] [SerializeField] private GameObject highScoreEntryRowPrefab;
        [SerializeField] private Button restartGameButton;

        [Inject] private IScoreSaver _scoreSaver;
        [Inject] private IPrefabPool _prefabPool;

        private void OnEnable()
        {
            SignalsHub.AddListener<GameFinishedSignal>(OnGameFinished);
            restartGameButton.onClick.AddListener(RestartGame);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<GameFinishedSignal>(OnGameFinished);
            restartGameButton.onClick.RemoveListener(RestartGame);
        }

        private void Start()
        {
            popup.SetActive(false);
        }

        private void OnGameFinished(GameFinishedSignal signal)
        {
            popup.SetActive(true);
            Refresh();
        }
        
        private void RestartGame()
        {
            popup.SetActive(false);
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        public void Refresh()
        {
            foreach (Transform c in contentParent) Destroy(c.gameObject);

            var entries = _scoreSaver.Entries;
            for (var i = 0; i < entries.Count; i++)
            {
                var row = _prefabPool.Spawn(highScoreEntryRowPrefab, contentParent).GetComponent<HighScoreEntryRow>();
                row.SetUp(i+1, entries[i]);
                // TODO: Highlight the current player's score
            }
        }

        // Buttons
        public void ClearAll()
        {
            _scoreSaver.ClearAll();
            Refresh();
        }

        public void ExportTxt()
        {
            _scoreSaver.SaveToTextFile();
        }
    }
}