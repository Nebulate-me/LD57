using System.Collections.Generic;
using System.Linq;
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
        
        private int currentTutorialStepIndex = 0;
        private IMaybe<TutorialStepConfig> shownTutorialStep = Maybe.Empty<TutorialStepConfig>();
        
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
            foreach (var tutorialStepConfig in tutorialSteps)
            {
                tutorialStepConfig.Content.SetActive(false);
            }
            ShowTutorialStep(tutorialSteps.First());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                HideShownTutorialStep();
            }
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
            if (!GetCurrentTutorialStep().TryGetValue(out var currentStep)) return;
            if (currentStep.ShowTrigger == TutorialStepTrigger.OnRoomCardSelected)
            {
                HideShownTutorialStep();
                ShowTutorialStep(currentStep);
            }
        }
        
        private void OnRoomPlaced(RoomPlacedSignal obj)
        {
            if (!GetCurrentTutorialStep().TryGetValue(out var currentStep)) return;
            if (currentStep.ShowTrigger == TutorialStepTrigger.OnRoomCardPlaced)
            {
                HideShownTutorialStep();
                ShowTutorialStep(currentStep);
            }
        }
        
        private void OnMissionCompleted(MissionCompletedSignal obj)
        {
            if (!GetCurrentTutorialStep().TryGetValue(out var currentStep)) return;
            if (currentStep.ShowTrigger == TutorialStepTrigger.OnMissionCompleted)
            {
                HideShownTutorialStep();
                ShowTutorialStep(currentStep);
            }
        }
        

        private IMaybe<TutorialStepConfig> GetCurrentTutorialStep()
        {
            return tutorialSteps.Count > currentTutorialStepIndex
                ? Maybe.Of(tutorialSteps[currentTutorialStepIndex])
                : Maybe.Empty<TutorialStepConfig>();
        }
        
        private void ShowTutorialStep(TutorialStepConfig tutorialStep)
        {
            tutorialStep.Content.SetActive(true);
            shownTutorialStep = Maybe.Of(tutorialStep);
            currentTutorialStepIndex++;
        }

        private void HideShownTutorialStep()
        {
            if (shownTutorialStep.TryGetValue(out var step))
            {
                step.Content.SetActive(false);
                shownTutorialStep = Maybe.Empty<TutorialStepConfig>();

                if (GetCurrentTutorialStep().TryGetValue(out var currentStep) && currentStep.ShowTrigger == TutorialStepTrigger.None)
                {
                    ShowTutorialStep(currentStep);
                }
            }
        }
    }
}
