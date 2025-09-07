using System.Linq;
using _Scripts.Player;
using _Scripts.Popups.HighScore;
using Signals;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Screens
{
    public class HighScoreScreenController : MonoBehaviour
    {
        [Header("UI")] [SerializeField] private Transform contentParent; 
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private Button returnButton;

        [Inject] private IPrefabPool _prefabPool;
        [Inject] private IPlayerProfileService _playerProfileService;
        [Inject] private IScreenManager _screenManager;

        private void Awake()
        {
            if (!contentParent) Debug.LogWarning("HighScoreScreenController: contentParent not set.");
            if (!rowPrefab) Debug.LogWarning("HighScoreScreenController: rowPrefab not set.");
        }

        private void OnEnable()
        {
            if (returnButton) returnButton.onClick.AddListener(OnReturnToMainMenuButtonClicked);
            
            SignalsHub.AddListener<PlayerProfilePopupClosedSignal>(OnPlayerProfilePopupClosed);
        }

        private void OnDisable()
        {
            if (returnButton) returnButton.onClick.RemoveListener(OnReturnToMainMenuButtonClicked);
            
            SignalsHub.RemoveListener<PlayerProfilePopupClosedSignal>(OnPlayerProfilePopupClosed);
        }
        

        private void Start()
        {
            Refresh();
        }

        private void Refresh()
        {
            for (var i = contentParent.childCount - 1; i >= 0; i--)
                Destroy(contentParent.GetChild(i).gameObject);

            var currentPlayer = _playerProfileService.CurrentPlayer;
            var players = _playerProfileService.Players
                .OrderByDescending(player => player.Score)
                .ThenBy(player => player.CreatedUtc)
                .ToList();
            
            for (var i = 0; i < players.Count; i++)
            {
                var player = players[i];
                var row = _prefabPool.Spawn(rowPrefab, contentParent).GetComponent<HighScoreEntryRow>();

                var isCurrent = currentPlayer != null && player.Id == currentPlayer.Id;

                row.SetUp(
                    index: i + 1,
                    entry: player,
                    isHighlighted: isCurrent
                );
            }
        }
        
        private void OnPlayerProfilePopupClosed(PlayerProfilePopupClosedSignal signal)
        {
            Refresh();
        }

        private void OnReturnToMainMenuButtonClicked()
        {
            _screenManager.GoToMainMenuScreen();
        }
    }
}