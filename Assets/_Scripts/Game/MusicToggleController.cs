using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game
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
        }

        private void Start()
        {
            UpdateIcon();
        }

        private void OnDisable()
        {
            musicToggleButton.onClick.RemoveListener(ToggleMusic);
        }

        private void ToggleMusic()
        {
            _soundManager.IsMusicOn = !_soundManager.IsMusicOn;
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            musicToggleIcon.sprite = _soundManager.IsMusicOn ? musicToggleOn : musicToggleOff;
        }
    }
}