using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Scripts.Game.Timer;
using _Scripts.Missions;
using _Scripts.Popups.GameFinished;
using _Scripts.Screens;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Popups.LevelFinished
{
    public class LevelFinishedScreenController : BasePopupController
    {
        private static readonly int OctopusHasTurnedAnimationKey = Animator.StringToHash("HasTurned");
        
        [Header("Functionality")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI currentPointsText;
        [SerializeField] private TextMeshProUGUI currentRankText;
        [SerializeField] private TextMeshProUGUI currentRankDescriptionText;

        [SerializeField] private Button continueButton;
        [SerializeField] private TextMeshProUGUI continueButtonText;
        [SerializeField] private Button backToMainMenuButton;
        
        [Header("Texts")]
        [SerializeField] private string levelCompleted = "Уровень пройден!";
        [SerializeField] private string allLevelsCompleted = "Поздравляем! Вы прошли игру!";
        [SerializeField] private string timeOut = "Время вышло!";

        [Header("Animations")] [SerializeField]
        private CanvasGroup bodyCanvasGroup;

        [SerializeField] private float bodyFadeDuration = 0.5f;

        [SerializeField] private CanvasGroup popupCanvasGroup;

        [SerializeField] private Animator fireworksAnimator;
        [SerializeField] private RectTransform octopusTransform;
        [SerializeField] private Animator octopusAnimator;
        [SerializeField] private Vector3 octopusInitialPosition;
        [SerializeField] private Vector3 octopusFinalPosition;
        [SerializeField] private float octopusMoveDuration = 1f;
        [SerializeField] private RectTransform sunglassesTransform;
        [SerializeField] private Vector3 sunglassesInitialPosition;
        [SerializeField] private Vector3 sunglassesFinalPosition;
        [SerializeField] private float sunglassesMoveDuration = 1f;
        [SerializeField] private CanvasGroup medalCanvaGroup;

        [Inject] private IScoreManager _scoreManager;
        [Inject] private IGameTimerController _gameTimerController;
        [Inject] private IScreenManager _screenManager;

        protected override PopupType Type => PopupType.LevelFinished;

        public void Start()
        {
            OnStart();
        }

        protected override void OnShowPopup()
        {
            base.OnShowPopup();
            
            _gameTimerController.PauseTimer();
            
            currentPointsText.text = $"Очки: {_scoreManager.Score}";
            var currentRank = _scoreManager.GetCurrentRank();
            currentRankText.text = currentRank.RankName;
            currentRankDescriptionText.text = currentRank.RankDescription;

            if (_scoreManager.IsGameFinished)
            {
                titleText.text = _scoreManager.GameFinishedReason switch
                {
                    GameFinishedReason.TimeOut => timeOut,
                    GameFinishedReason.AllLevelsCompleted => allLevelsCompleted,
                    _ => throw new ArgumentOutOfRangeException()
                };
                continueButtonText.text = "Рекорды";
            }
            else
            {
                titleText.text = levelCompleted;
                continueButtonText.text = "Продолжить";
            }
            
            _ = AnimateShowPopup();
        }

        protected override void OnHidePopup()
        {
            base.OnHidePopup();

            bodyCanvasGroup.alpha = 0;
            _gameTimerController.StartTimer();
        }

        protected override void SetupSubscriptions()
        {
            base.SetupSubscriptions();

            continueButton.onClick.AddListener(OnContinueButtonClicked);
            backToMainMenuButton.onClick.AddListener(OnBackToMainMenuButtonClicked);
        }

        protected override void DisposeSubscriptions()
        {
            base.DisposeSubscriptions();

            continueButton.onClick.RemoveListener(OnContinueButtonClicked);
            backToMainMenuButton.onClick.RemoveListener(OnBackToMainMenuButtonClicked);
        }

        private void OnContinueButtonClicked()
        {
            if (_scoreManager.IsGameFinished)
            {
                _screenManager.GoToHighScoreScreen();
                return;
            }

            HidePopup();
            SignalsHub.DispatchAsync(new StartNextLevelSignal());
        }

        private void OnBackToMainMenuButtonClicked()
        {
            _screenManager.GoToMainMenuScreen();
        }

        private async UniTask AnimateShowPopup()
        {
            var halfOctopusMoveDuration = octopusMoveDuration / 2f;

            bodyCanvasGroup.alpha = 0;
            popupCanvasGroup.alpha = 0;
            octopusTransform.anchoredPosition = octopusInitialPosition;
            octopusAnimator.SetBool(OctopusHasTurnedAnimationKey, false);
            sunglassesTransform.anchoredPosition = sunglassesInitialPosition;
            medalCanvaGroup.alpha = 0;
            fireworksAnimator.gameObject.SetActive(true);

            await bodyCanvasGroup.DOFade(1f, bodyFadeDuration).AsyncWaitForCompletion();
            var octopusMove = octopusTransform.DOAnchorPos(octopusFinalPosition, octopusMoveDuration)
                .AsyncWaitForCompletion().AsUniTask();
            await UniTask.Delay(TimeSpan.FromSeconds(halfOctopusMoveDuration));
            octopusAnimator.SetBool(OctopusHasTurnedAnimationKey, true);
            var sunglassesMove = sunglassesTransform.DOAnchorPos(sunglassesFinalPosition, sunglassesMoveDuration)
                .AsyncWaitForCompletion().AsUniTask();
            await UniTask.Delay(TimeSpan.FromSeconds(halfOctopusMoveDuration));
            var popupFade = popupCanvasGroup.DOFade(1f, sunglassesMoveDuration - halfOctopusMoveDuration)
                .AsyncWaitForCompletion().AsUniTask();
            fireworksAnimator.gameObject.SetActive(false);

            await UniTask.WhenAll(new List<UniTask> {octopusMove, sunglassesMove, popupFade});
            await medalCanvaGroup.DOFade(1f, sunglassesMoveDuration - halfOctopusMoveDuration)
                .AsyncWaitForCompletion().AsUniTask();
        }
    }
}