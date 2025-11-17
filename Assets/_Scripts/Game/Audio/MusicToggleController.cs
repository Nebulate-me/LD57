using Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game.Audio
{
    public class MusicToggleController : MonoBehaviour
    {
        [SerializeField] private Button musicToggleButton;
        [SerializeField] private Image musicToggleIcon;
        [SerializeField] private Sprite musicToggleOn;
        [SerializeField] private Sprite musicToggleOff;
        
        [Inject] private ISoundManager _soundManager;

        private void OnEnable()
        {
            musicToggleButton.onClick.AddListener(ToggleMusic);
            SignalsHub.AddListener<UpdateAudioIconsSignal>(UpdateIcon);
        }

        private void Start()
        {
            UpdateIcon();
        }

        private void OnDisable()
        {
            musicToggleButton.onClick.RemoveListener(ToggleMusic);
            SignalsHub.RemoveListener<UpdateAudioIconsSignal>(UpdateIcon);
        }

        private void ToggleMusic()
        {
            _soundManager.IsMusicEnabled = !_soundManager.IsMusicEnabled;
            UpdateIcon();
        }
        
        private void UpdateIcon(UpdateAudioIconsSignal signal)
        {
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            musicToggleIcon.sprite = _soundManager.IsMusicEnabled ? musicToggleOn : musicToggleOff;
        }
    }
}