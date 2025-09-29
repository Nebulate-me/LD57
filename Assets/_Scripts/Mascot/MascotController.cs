using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Cards;
using _Scripts.Game.Timer;
using _Scripts.Missions;
using _Scripts.Missions.Apartment;
using _Scripts.Rooms;
using _Scripts.Screens;
using ModestTree;
using Signals;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Mascot
{
    public class MascotPopupController : MonoBehaviour
    {
        [SerializeField] private GameObject middleMascotPopup;
        [SerializeField] private CanvasGroup mascotBubbleCanvasGroup;
        [SerializeField] private GameObject aboveMascotPopup;
        [SerializeField] private CanvasGroup mascotCharacterCanvasGroup;
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI shownBubbleText;
        [SerializeField] private TextMeshProUGUI contentBubbleText;
        [SerializeField] private Button clickCatcher;

        [Header("Behavior")] [SerializeField] private float fadeDuration = 0.18f;
        [SerializeField] private bool useTypewriter = true;
        [SerializeField] private float charsPerSecond = 55f;

        [Header("Tutorial Targets")] [SerializeField]
        private MascotTutorialTopPanelTargetToGameObjectDictionary topPanelTutorialTargets;

        [SerializeField] private MascotTutorialBottomPanelTargetToGameObjectDictionary bottomPanelTutorialTargets;
        [SerializeField] private MascotTutorialBuildingTargetToGameObjectDictionary buildingTutorialTargets;

        [Header("Messages")] [SerializeField] private MascotTutorialConfig tutorialConfig;

        [Inject] private IScoreManager _scoreManager;
        [Inject] private IGameTimerController _gameTimerController;
        [Inject] private IHandManager _handManager;
        [Inject] private IScreenManager _screenManager;
        [Inject] private IDungeonGridManager _gridManager;

        private Coroutine showRoutine;
        private Coroutine typeRoutine;

        private readonly List<MascotTutorialStepConfig> _stepConfigs = new();
        private int _activeStepIndex;
        [ShowInInspector, ReadOnly] private bool _isHidden = true;

        private void Awake()
        {
            _isHidden = true;
            middleMascotPopup.SetActive(false);
            aboveMascotPopup.SetActive(false);
        }

        private void OnEnable()
        {
            SignalsHub.AddListener<RoomCardSelectedSignal>(OnRoomCardSelected);
            SignalsHub.AddListener<RoomPlacedSignal>(OnRoomPlaced);
            SignalsHub.AddListener<ApartmentMissionCompletedSignal>(OnMissionCompleted);

            if (clickCatcher) clickCatcher.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<RoomCardSelectedSignal>(OnRoomCardSelected);
            SignalsHub.RemoveListener<RoomPlacedSignal>(OnRoomPlaced);
            SignalsHub.RemoveListener<ApartmentMissionCompletedSignal>(OnMissionCompleted);

            if (clickCatcher) clickCatcher.onClick.RemoveListener(OnClicked);
        }

        private void Start()
        {
            ShowSequence(tutorialConfig.Steps);
        }

        /// <summary>
        ///     Starts showing a sequence of phrases (fades in once, then click-through).
        /// </summary>
        public void ShowSequence(List<MascotTutorialStepConfig> stepConfigs)
        {
            _stepConfigs.Clear();
            if (stepConfigs == null) return;

            _stepConfigs.AddRange(stepConfigs);
            if (_stepConfigs.IsEmpty())
            {
                _scoreManager.StartGame();
                return;
            }

            _activeStepIndex = 0;

            ShowPopup();
            
            if (GetCurrentStepConfig(out var currentStepConfig))
            {
                SetPhrase(currentStepConfig.Phrase, true);
                SetTutorialTarget(currentStepConfig);
                SetTutorialAction(currentStepConfig);   
            }
        }

        private void ShowPopup()
        {
            if (showRoutine != null) StopCoroutine(showRoutine);
            if (typeRoutine != null) StopCoroutine(typeRoutine);
            
            middleMascotPopup.SetActive(true);
            aboveMascotPopup.SetActive(true);
            var canvasGroups = new List<CanvasGroup> {mascotBubbleCanvasGroup, mascotCharacterCanvasGroup};
            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            showRoutine = StartCoroutine(FadeInThenType());
        }

        public void Hide()
        {
            if (showRoutine != null) StopCoroutine(showRoutine);
            if (typeRoutine != null) StopCoroutine(typeRoutine);
            StartCoroutine(FadeOutAndDisable());
            SignalsHub.DispatchAsync(new TutorialHiddenSignal());
        }

        private bool GetCurrentStepConfig(out MascotTutorialStepConfig stepConfig)
        {
            if (_activeStepIndex < 0 || _activeStepIndex >= _stepConfigs.Count)
            {
                stepConfig = null;
                return false;
            }

            stepConfig = _stepConfigs[_activeStepIndex];
            return true;
        }

        private async void OnRoomCardSelected(RoomCardSelectedSignal signal)
        {
            if (!GetCurrentStepConfig(out var stepConfig) ||
                stepConfig.ActionType != MascotTutorialActionType.RoomCardSelected ||
                signal.RoomCard == null ||
                !stepConfig.SelectedRoomCard.IsEqual(signal.RoomCard)) return;

            StartCoroutine(ShowPopupAndNextStepCoroutine());
        }

        private async void OnRoomPlaced(RoomPlacedSignal signal)
        {
            // TODO: Check if the correct room position is used
            if (!GetCurrentStepConfig(out var stepConfig) ||
                stepConfig.ActionType != MascotTutorialActionType.RoomPlaced ||
                signal.Room == null ||
                !stepConfig.SelectedRoomCard.IsEqual(signal.Room)) return;
         
            _gridManager.HideTutorialGhostRoom();
            StartCoroutine(ShowPopupAndNextStepCoroutine());
        }
        
        private void OnMissionCompleted(ApartmentMissionCompletedSignal obj)
        {
            if (!GetCurrentStepConfig(out var stepConfig) ||
                stepConfig.ActionType != MascotTutorialActionType.MissionCompleted) return;
            
            StartCoroutine(ShowPopupAndNextStepCoroutine());
        }
        
        private IEnumerator ShowPopupAndNextStepCoroutine()
        {
            if (showRoutine != null) StopCoroutine(showRoutine);
            if (typeRoutine != null) StopCoroutine(typeRoutine);
            yield return StartCoroutine(FadeOutAndDisable());
            SignalsHub.DispatchAsync(new TutorialHiddenSignal());
            
            yield return new WaitForSeconds(0.05f);
            ShowNextStep();
            ShowPopup();
        }

        private void OnClicked()
        {
            // If typewriter hasn’t finished, complete immediately.
            if (useTypewriter && shownBubbleText.maxVisibleCharacters < shownBubbleText.text.Length)
            {
                shownBubbleText.maxVisibleCharacters = shownBubbleText.text.Length;
                return;
            }

            if (!GetCurrentStepConfig(out var stepConfig))
            {
                Hide();
                return;
            }

            if (stepConfig.ActionType == MascotTutorialActionType.ClickAny)
            {
                if (stepConfig.TargetType == MascotTutorialTargetType.Building && stepConfig.BuildingPanelTargetType ==
                    MascotTutorialBuildingTargetType.HighlightWindows)
                {
                    SignalsHub.DispatchAsync(new UnhighlightWindowsSignal());
                }
                
                ShowNextStep();
                return;
            }

            if (stepConfig.ActionType == MascotTutorialActionType.FinishTutorial)
            {
                _screenManager.GoToMainMenuScreen();
                return;
            }

            SetTutorialTarget(stepConfig);
            _gameTimerController.ResumeTimer(isTutorial: true);
            Hide();
        }

        private void ShowNextStep()
        {
            // Otherwise go to next phrase, or finish if this was the last.
            if (_activeStepIndex < _stepConfigs.Count - 1)
            {
                _activeStepIndex++;
                // stop any previous typing coroutine
                if (typeRoutine != null) StopCoroutine(typeRoutine);
                if (GetCurrentStepConfig(out var stepConfig))
                {
                    SetPhrase(stepConfig.Phrase, true);
                    SetTutorialTarget(stepConfig);
                    SetTutorialAction(stepConfig);
                }

                if (useTypewriter)
                    typeRoutine = StartCoroutine(Typewriter(shownBubbleText, charsPerSecond));
            }
            else
            {
                // Last phrase → fade out and start the game
                Hide();
            }
        }

        private void SetPhrase(string phrase, bool resetVisible)
        {
            contentBubbleText.text = phrase;
            shownBubbleText.text = phrase;
            if (resetVisible) shownBubbleText.maxVisibleCharacters = 0;
            mascotBubbleCanvasGroup.alpha = phrase.Trim().IsEmpty() ? 0f : 1f;
        }

        private void ClearTutorialTarget()
        {
            foreach (var topPanelTutorialTarget in topPanelTutorialTargets.Values)
                topPanelTutorialTarget.SetActive(false);

            foreach (var topPanelTutorialTarget in bottomPanelTutorialTargets.Values)
                topPanelTutorialTarget.SetActive(false);

            foreach (var buildingTutorialTarget in buildingTutorialTargets.Values)
                buildingTutorialTarget.SetActive(false);
        }

        private void SetTutorialTarget(MascotTutorialStepConfig stepConfig)
        {
            ClearTutorialTarget();

            switch (stepConfig.TargetType)
            {
                case MascotTutorialTargetType.TopPanel:
                {
                    if (topPanelTutorialTargets.TryGetValue(stepConfig.TopPanelTargetType, out var stepTarget))
                        stepTarget.SetActive(true);
                    break;
                }
                case MascotTutorialTargetType.BottomPanel:
                {
                    if (bottomPanelTutorialTargets.TryGetValue(stepConfig.BottomPanelTargetType, out var stepTarget))
                    {
                        stepTarget.SetActive(true);
                        if (stepConfig.BottomPanelTargetType == MascotTutorialBottomPanelTargetType.RoomCard &&
                            _handManager.TryGetCardView(stepConfig.BottomPanelTargetRoom, out var handRoomCard))
                            stepTarget.transform.position = new Vector3(
                                handRoomCard.transform.position.x,
                                stepTarget.transform.position.y,
                                stepTarget.transform.position.z
                            );
                    }
                    break;
                }
                case MascotTutorialTargetType.Building:
                {
                    if (buildingTutorialTargets.TryGetValue(stepConfig.BuildingPanelTargetType, out var stepTarget))
                    {
                        stepTarget.SetActive(true);
                    }
                    else if (stepConfig.BuildingPanelTargetType == MascotTutorialBuildingTargetType.Space)
                    {
                        _gridManager.ShowTutorialGhostRoom(stepConfig.BuildingSpaceRoomPosition, stepConfig.BuildingSpaceRoom.ToDto(), stepConfig.BuildingSpaceRoomDirection);
                    } 
                    else if (stepConfig.BuildingPanelTargetType == MascotTutorialBuildingTargetType.HighlightWindows)
                    {
                        SignalsHub.DispatchAsync(new HighlightWindowsSignal());
                    }
                    break;
                }
                case MascotTutorialTargetType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetTutorialAction(MascotTutorialStepConfig stepConfig)
        {
            _gameTimerController.PauseTimer(isTutorial: true);
        }

        private IEnumerator FadeInThenType()
        {
            var canvasGroups = new List<CanvasGroup> {mascotCharacterCanvasGroup};
            if (GetCurrentStepConfig(out var stepConfig) && !stepConfig.Phrase.Trim().IsEmpty()) 
                canvasGroups.Add(mascotBubbleCanvasGroup);
            
            // Fade in
            var t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                foreach (var canvasGroup in canvasGroups)
                    canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, t / fadeDuration);
                yield return null;
            }

            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            // Typewriter for first phrase
            if (useTypewriter) typeRoutine = StartCoroutine(Typewriter(shownBubbleText, charsPerSecond));
            _isHidden = false;
        }

        private IEnumerator FadeOutAndDisable()
        {
            var canvasGroups = new List<CanvasGroup> {mascotBubbleCanvasGroup, mascotCharacterCanvasGroup};
            foreach (var canvasGroup in canvasGroups)
                canvasGroup.interactable = false;

            var t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                foreach (var canvasGroup in canvasGroups)
                    canvasGroup.alpha = Mathf.SmoothStep(1f, 0f, t / fadeDuration);
                yield return null;
            }

            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
            }

            middleMascotPopup.SetActive(false);
            aboveMascotPopup.SetActive(false);
            _isHidden = true;
        }

        private static IEnumerator Typewriter(TMP_Text label, float cps)
        {
            label.maxVisibleCharacters = 0;
            var total = label.text.Length;
            var acc = 0f;
            while (label.maxVisibleCharacters < total)
            {
                acc += Time.unscaledDeltaTime * cps;
                var visible = Mathf.Min(total, Mathf.FloorToInt(acc));
                if (visible != label.maxVisibleCharacters)
                    label.maxVisibleCharacters = visible;
                yield return null;
            }
        }
    }
}