namespace _Scripts.Game.Audio
{
    public interface ISoundManager
    {
        bool IsMusicEnabled { get; set; }
        bool IsSoundEnabled { get; set; }
        void PlaySound(SoundType type);
    }
}