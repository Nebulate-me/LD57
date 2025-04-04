using System;
using AudioTools.Sound;
using Plugins.Sirenix.Odin_Inspector.Modules;
using Signals;
using UnityEngine;
using Utilities;
using Zenject;

namespace _Scripts.Utils.AudioTool.Sounds
{
    public class SoundController : MonoBehaviour
    {
        [SerializeField] private SoundNameToSampleDictionary soundSamples = new();

        [Inject] private ISoundManager<SoundType> soundManager;

        private void OnEnable()
        {
            SignalsHub.AddListener<PlaySoundSignal>(OnPlaySoundSignal);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<PlaySoundSignal>(OnPlaySoundSignal);
        }

        private void OnPlaySoundSignal(PlaySoundSignal signal)
        {
            PlaySound(signal.Name);
        }

        private void PlaySound(SoundName soundName)
        {
            var sound = soundSamples.SafeGet(soundName);
            soundManager.Play(sound);
        }

        [Serializable]
        public class SoundNameToSampleDictionary : UnitySerializedDictionary<SoundName, SoundSample>
        {
        }
    }
}