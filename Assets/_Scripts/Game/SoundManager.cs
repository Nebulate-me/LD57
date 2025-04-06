using System;
using UnityEngine;

namespace _Scripts.Game
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        [SerializeField] private AudioSource source;

        [SerializeField] private AudioClip placeRoomSound;
        [SerializeField] private AudioClip completeMissionSound;
        [SerializeField] private AudioClip defeatSound;
        
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}