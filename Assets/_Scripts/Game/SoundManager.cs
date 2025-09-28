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
        [SerializeField] private AudioClip redrawHandSound;
        [SerializeField] private AudioClip undoPlaceRoomSound;
        [SerializeField] private AudioClip completeLevelSound;
        [SerializeField] private AudioClip completeGameSound;

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