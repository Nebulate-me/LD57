using _Scripts.Game.Timer;
using _Scripts.Missions;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.TimeManagement;
using Zenject;

namespace _Scripts.Popups.LevelFinished
{
    public class LevelFinishedPopupController : BasePopupController
    {
        [SerializeField] private TextMeshProUGUI currentRankText;
        [SerializeField] private Button nextLevelButton;
        
        [Inject] private IScoreManager _scoreManager;
        [Inject] private IGameTimerController _gameTimerController;
        
        protected override PopupType Type => PopupType.LevelFinished;
        
        public void Start()
        {
            OnStart();
        }

        protected override void OnShowPopup()
        {
            base.OnShowPopup();
            currentRankText.text = _scoreManager.GetCurrentRank().RankName;
            _gameTimerController.PauseTimer();
        }

        protected override void OnHidePopup()
        {
            base.OnHidePopup();
            _gameTimerController.StartTimer();
        }

        protected override void SetupSubscriptions()
        {
            base.SetupSubscriptions();
            
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }

        protected override void DisposeSubscriptions()
        {
            base.DisposeSubscriptions();
            
            nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClicked);
        }
        
        private void OnNextLevelButtonClicked()
        {
            HidePopup();
            SignalsHub.DispatchAsync(new StartNextLevelSignal());
        }
    }
}