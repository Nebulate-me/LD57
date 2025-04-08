using System;
using UnityEngine;

namespace _Scripts.Game
{
    [Serializable]
    public class TutorialStepConfig
    {
        [SerializeField] private GameObject content;
        [SerializeField] private TutorialStepTrigger showTrigger;
        [SerializeField] private TutorialStepTrigger hideTrigger;
        
        public GameObject Content => content;
        public TutorialStepTrigger ShowTrigger => showTrigger;
        public TutorialStepTrigger HideTrigger => hideTrigger;
    }
}