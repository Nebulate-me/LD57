using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Mascot
{
    [Serializable]
    public class MascotTutorialStepConfig
    {
        [SerializeField, TextArea(2, 5)] private string phrase = string.Empty;
        [SerializeField] private MascotTutorialTargetType targetType;
        [ShowIf(nameof(IsTargetTopPanel)), SerializeField] private MascotTutorialTopPanelTargetType topPanelTargetType;
        [ShowIf(nameof(IsTargetBottomPanel)), SerializeField] private MascotTutorialBottomPanelTargetType bottomPanelTargetType;

        [SerializeField] private MascotTutorialActionType actionType;
        
        private bool IsTargetTopPanel => targetType == MascotTutorialTargetType.TopPanel;
        private bool IsTargetBottomPanel => targetType == MascotTutorialTargetType.BottomPanel;
        
        public string Phrase => phrase;
        public MascotTutorialTargetType TargetType => targetType;
        public MascotTutorialTopPanelTargetType TopPanelTargetType => topPanelTargetType;
        public MascotTutorialActionType ActionType => actionType;
    }
}