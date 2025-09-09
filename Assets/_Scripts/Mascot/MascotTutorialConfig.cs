using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Mascot
{
    [CreateAssetMenu(menuName = "LD57/Create MascotTutorialConfig", fileName = "MascotTutorialConfig", order = 0)]
    public class MascotTutorialConfig : ScriptableObject
    {
        [SerializeField] private List<MascotTutorialStepConfig> steps =  new List<MascotTutorialStepConfig>();
        
        public List<MascotTutorialStepConfig> Steps => steps;
    }
}