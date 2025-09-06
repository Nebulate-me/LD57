using _Scripts.Game;
using _Scripts.Missions;
using _Scripts.Player;
using _Scripts.Score;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Popups.HighScore
{
    public class HighScorePopupController : BasePopupController
    {
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject highScoreEntryRowPrefab;
        [SerializeField] private Button restartGameButton;
        [SerializeField] private int maxScoreRows = 10; // TODO: Scrollable view
        
        [Inject] private IPlayerProfileService _playerProfileService;
        [Inject] private IPrefabPool _prefabPool;

        protected override PopupType Type => PopupType.HighScore;
        
        public void Start()
        {
            OnStart();
        }
        
        protected override void SetupSubscriptions()
        {
            base.SetupSubscriptions();
            
            restartGameButton.onClick.AddListener(RestartGame);
        }

        protected override void DisposeSubscriptions()
        {
            base.DisposeSubscriptions();
            
            restartGameButton.onClick.RemoveListener(RestartGame);
        }

        protected override void OnShowPopup()
        {
            base.OnShowPopup();
            Refresh();
        }

        private void RestartGame()
        {
            HidePopup();
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        public void Refresh()
        {
            foreach (Transform c in contentParent) Destroy(c.gameObject);

            var players = _playerProfileService.Players;
            var entryCount = Mathf.Min(players.Count, maxScoreRows);
            for (var i = 0; i < entryCount; i++)
            {
                var row = _prefabPool.Spawn(highScoreEntryRowPrefab, contentParent).GetComponent<HighScoreEntryRow>();
                row.SetUp(i + 1, players[i], players[i].Name == _playerProfileService.CurrentPlayer.Name);
            }
        }
    }
}