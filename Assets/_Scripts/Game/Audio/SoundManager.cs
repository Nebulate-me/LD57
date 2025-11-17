using System;
using Signals;
using UnityEngine;
using UnityEngine.Audio;

namespace _Scripts.Game.Audio
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource source;

        [SerializeField] private AudioClip placeRoomSound;
        [SerializeField] private AudioClip completeMissionSound;
        [SerializeField] private AudioClip defeatSound;
        [SerializeField] private AudioClip redrawHandSound;
        [SerializeField] private AudioClip undoPlaceRoomSound;
        [SerializeField] private AudioClip completeLevelSound;
        [SerializeField] private AudioClip completeGameSound;
        
        private const string MUSIC_ENABLED_KEY = "MusicEnabled";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        
        private const string SOUND_ENABLED_KEY = "SoundEnabled";
        private const string SOUND_VOLUME_KEY = "SoundVolume";

        private const float MINIMUM_VOLUME = -80f;
        private const float MAXIMUM_VOLUME = 0f;

        private void Start()
        {
            IsMusicEnabled = PlayerPrefs.GetFloat(MUSIC_ENABLED_KEY, 1f) > 0f;
            MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            
            IsSoundEnabled = PlayerPrefs.GetFloat(SOUND_ENABLED_KEY, 1f) > 0f;
            SoundVolume = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 1f);
            
            SignalsHub.DispatchAsync(new UpdateAudioIconsSignal());
        }

        public bool IsMusicEnabled
        {
            get => PlayerPrefs.GetFloat(MUSIC_ENABLED_KEY) > 0f;
            set
            {
                PlayerPrefs.SetFloat(MUSIC_ENABLED_KEY, value ? 1f : 0f);
                audioMixer.SetFloat(MUSIC_VOLUME_KEY, value 
                    ? CalculateVolume(PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY)) 
                    : MINIMUM_VOLUME);
            }
        }

        public float MusicVolume
        {
            get => PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
            set {
                PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
                audioMixer.SetFloat(MUSIC_VOLUME_KEY, IsMusicEnabled ? CalculateVolume(value) : MINIMUM_VOLUME);
            }
        }
        
        public bool IsSoundEnabled
        {
            get => PlayerPrefs.GetFloat(SOUND_ENABLED_KEY) > 0f;
            set
            {
                PlayerPrefs.SetFloat(SOUND_ENABLED_KEY, value ? 1f : 0f);
                audioMixer.SetFloat(SOUND_VOLUME_KEY, value 
                    ? CalculateVolume(PlayerPrefs.GetFloat(SOUND_VOLUME_KEY)) 
                    : MINIMUM_VOLUME);
            }
        }
        
        public float SoundVolume
        {
            get => PlayerPrefs.GetFloat(SOUND_VOLUME_KEY);
            set {
                PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, value);
                audioMixer.SetFloat(SOUND_VOLUME_KEY, IsSoundEnabled ? CalculateVolume(value) : MINIMUM_VOLUME);
            }
        }
        
        private float CalculateVolume(float volumePercentage)
        {
            var clampedPercentage = Mathf.Clamp01(volumePercentage);
            return MINIMUM_VOLUME + clampedPercentage * (MAXIMUM_VOLUME - MINIMUM_VOLUME);
        }

        public void PlaySound(SoundType type)
        {
            switch (type)
            {
                case SoundType.None:
                    break;
                case SoundType.PlaceRoom:
                    source.PlayOneShot(placeRoomSound);
                    break;
                case SoundType.CompleteMission:
                    source.PlayOneShot(completeMissionSound);
                    break;
                case SoundType.Defeat:
                    source.PlayOneShot(defeatSound);
                    break;
                case SoundType.RedrawHand:
                    source.PlayOneShot(redrawHandSound);
                    break;
                case SoundType.UndoPlaceRoom:
                    source.PlayOneShot(undoPlaceRoomSound);
                    break;
                case SoundType.CompleteLevel:
                    source.PlayOneShot(completeLevelSound);
                    break;
                case SoundType.CompleteGame:
                    source.PlayOneShot(completeGameSound);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}