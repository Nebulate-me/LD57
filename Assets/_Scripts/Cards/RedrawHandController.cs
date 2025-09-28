using System;
using _Scripts.Game;
using _Scripts.Game.Timer;
using _Scripts.Missions;
using Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Cards
{
    public class RedrawHandController : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Button redrawHandButton;
        [SerializeField] private int redrawCostScore = 5;

        [Space, SerializeField] private bool disablePermanently = false; // FIXME: Not usable in the Tutorial
        [ReadOnly, ShowInInspector] private bool _isDisabled = true;

        [Inject] private IHandManager _handManager;
        [Inject] private IScoreManager _scoreManager;
        [Inject] private ISoundManager _soundManager;

        private void OnEnable()
        {
            SignalsHub.AddListener<GameTimerStartedSignal>(OnGameTimerStarted);
            SignalsHub.AddListener<GameTimerPausedSignal>(OnGameTimerPaused);
            
            redrawHandButton.onClick.AddListener(RedrawHand);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<GameTimerStartedSignal>(OnGameTimerStarted);
            SignalsHub.RemoveListener<GameTimerPausedSignal>(OnGameTimerPaused);
            
            redrawHandButton.onClick.RemoveListener(RedrawHand);
        }

        private void Start()
        {
            SetDisabled(true);
        }

        private void OnGameTimerStarted(GameTimerStartedSignal signal)
        {
            SetDisabled(false);
        }
        
        private void OnGameTimerPaused(GameTimerPausedSignal signal)
        {
            SetDisabled(true);
        }

        private void SetDisabled(bool disabled)
        {
            _isDisabled = disabled;
            redrawHandButton.interactable = !disabled && !disablePermanently;
        }
        
        private void RedrawHand()
        {
            if (_isDisabled)  return;
            
            _soundManager.PlaySound(SoundType.RedrawHand);
            _handManager.RedrawRoomHand();
            _scoreManager.SubtractScore(redrawCostScore);
        }
    }
}