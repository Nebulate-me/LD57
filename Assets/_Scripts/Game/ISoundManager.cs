namespace _Scripts.Game
{
    public interface ISoundManager
    {
        bool IsMusicOn { get; set; }
        void PlaySound(SoundType type);
    }
}