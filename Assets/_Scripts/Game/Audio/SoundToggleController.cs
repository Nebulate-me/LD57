using Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game.Audio
{
    public class SoundToggleController: MonoBehaviour
    {
        [SerializeField] private Button soundToggleButton;
        [SerializeField] private Image soundToggleIcon;
        [SerializeField] private Sprite soundToggleOn;
        [SerializeField] private Sprite soundToggleOff;
        
        [Inject] private ISoundManager _soundManager;

        private void OnEnable()
        {
            soundToggleButton.onClick.AddListener(ToggleSound);
            SignalsHub.AddListener<UpdateAudioIconsSignal>(UpdateIcon);
        }

        private void Start()
        {
            UpdateIcon();
        }

        private void OnDisable()
        {
            soundToggleButton.onClick.RemoveListener(ToggleSound);
            SignalsHub.RemoveListener<UpdateAudioIconsSignal>(UpdateIcon);
        }

        private void ToggleSound()
        {
            _soundManager.IsSoundEnabled = !_soundManager.IsSoundEnabled;
            UpdateIcon();
        }
        
        private void UpdateIcon(UpdateAudioIconsSignal signal)
        {
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            soundToggleIcon.sprite = _soundManager.IsSoundEnabled ? soundToggleOn : soundToggleOff;
        }
    }
}