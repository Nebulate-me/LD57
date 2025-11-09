using System;
using TMPro;
using UnityEngine;

namespace _Scripts.Game
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class GameNameTextController : MonoBehaviour
    {
        [SerializeField] private bool spacesAsLineBreaks = false;
        
        private TextMeshProUGUI _gameNameText;
        private void Awake()
        {
            _gameNameText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            var gameName = spacesAsLineBreaks ? Application.productName.Replace(" ", "\n") : Application.productName;
            _gameNameText.text = gameName;
        }
    }
}