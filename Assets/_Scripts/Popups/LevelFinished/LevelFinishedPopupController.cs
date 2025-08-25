using _Scripts.Missions;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Popups.LevelFinished
{
    public class LevelFinishedPopupController : BasePopupController
    {
        [SerializeField] private TextMeshProUGUI currentRankText;
        [SerializeField] private Button nextLevelButton;
        
        [Inject] private IScoreManager _scoreManager;
        protected override PopupType Type => PopupType.LevelFinished;
        
        public void Start()
        {
            OnStart();
        }

        protected override void OnShowPopup()
        {
            base.OnShowPopup();
            currentRankText.text = _scoreManager.RankName;
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