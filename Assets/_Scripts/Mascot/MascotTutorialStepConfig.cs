using System;
using UnityEngine;

namespace _Scripts.Mascot
{
    [Serializable]
    public class MascotTutorialStepConfig
    {
        [SerializeField, TextArea(2, 5)] private string phrase = string.Empty;
        [SerializeField] private MascotTutorialTargetType targetType;
        [SerializeField] private MascotTutorialActionType actionType;
        
        public string Phrase => phrase;
        public MascotTutorialTargetType TargetType => targetType;
        public MascotTutorialActionType ActionType => actionType;
    }
}