using System.Collections.Generic;
using _Scripts.Missions;
using _Scripts.Rooms;
using _Scripts.Utils;
using Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities.Monads;

namespace _Scripts.Game
{
    public class TutorialController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject helpPopover;
        [SerializeField] private List<TutorialStepConfig> tutorialSteps = new();

        private int nextTutorialStepIndex;
        private IMaybe<TutorialStepConfig> maybeShownTutorialStep = Maybe.Empty<TutorialStepConfig>();
        
        private const string TutorialCompletedKey = "TutorialCompleted";
        private const int TutorialCompletedValue = 1;

        private void OnEnable()
        {
            SignalsHub.AddListener<RoomCardSelectedSignal>(OnRoomCardSelected);
            SignalsHub.AddListener<RoomPlacedSignal>(OnRoomPlaced);
            SignalsHub.AddListener<MissionCompletedSignal>(OnMissionCompleted);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<RoomCardSelectedSignal>(OnRoomCardSelected);
            SignalsHub.RemoveListener<RoomPlacedSignal>(OnRoomPlaced);
            SignalsHub.RemoveListener<MissionCompletedSignal>(OnMissionCompleted);
        }

        private void Start()
        {
            helpPopover.SetActive(false);
            foreach (var tutorialStepConfig in tutorialSteps) tutorialStepConfig.Content.SetActive(false);

            if (!PlayerPrefs.HasKey(TutorialCompletedKey) || PlayerPrefs.GetInt(TutorialCompletedKey) != TutorialCompletedValue)
            {
                OnTriggerEvent(TutorialStepTrigger.None);   
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) OnTriggerEvent(TutorialStepTrigger.OnClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            helpPopover.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            helpPopover.SetActive(false);
        }

        private void OnRoomCardSelected(RoomCardSelectedSignal obj)
        {
            OnTriggerEvent(TutorialStepTrigger.OnRoomCardSelected);
        }

        private void OnRoomPlaced(RoomPlacedSignal obj)
        {
            OnTriggerEvent(TutorialStepTrigger.OnRoomCardPlaced);
        }

        private void OnMissionCompleted(MissionCompletedSignal obj)
        {
            OnTriggerEvent(TutorialStepTrigger.OnMissionCompleted);
        }

        private void OnTriggerEvent(TutorialStepTrigger triggerType)
        {
            if (maybeShownTutorialStep.TryGetValue(out var shownTutorialStep) &&
                shownTutorialStep.HideTrigger == triggerType)
            {
                HideTutorialStep(shownTutorialStep);
                maybeShownTutorialStep = Maybe.Empty<TutorialStepConfig>();
            }

            if (GetTutorialStep(nextTutorialStepIndex).TryGetValue(out var nextTutorialStep)
                && nextTutorialStep.ShowTrigger == triggerType)
            {
                ShowTutorialStep(nextTutorialStep);
                maybeShownTutorialStep = Maybe.Of(nextTutorialStep);
                nextTutorialStepIndex++;
                if (!IsValidStepIndex(nextTutorialStepIndex))
                {
                    PlayerPrefs.SetInt(TutorialCompletedKey, TutorialCompletedValue);
                }
            }
        }

        private IMaybe<TutorialStepConfig> GetTutorialStep(int tutorialStepIndex)
        {
            return IsValidStepIndex(tutorialStepIndex)
                ? Maybe.Of(tutorialSteps[nextTutorialStepIndex])
                : Maybe.Empty<TutorialStepConfig>();
        }

        private void ShowTutorialStep(TutorialStepConfig tutorialStep)
        {
            tutorialStep.Content.SetActive(true);
        }

        private void HideTutorialStep(TutorialStepConfig tutorialStep)
        {
            tutorialStep.Content.SetActive(false);
        }

        private bool IsValidStepIndex(int tutorialStepIndex)
        {
            return tutorialStepIndex >= 0 && tutorialStepIndex < tutorialSteps.Count;
        }
    }
}