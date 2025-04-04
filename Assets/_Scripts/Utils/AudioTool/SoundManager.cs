using _Scripts.Utils.AudioTool.Sounds;
using AudioTools.Sound;
using Utilities.Prefabs;
using Utilities.RandomService;

namespace _Scripts.Utils.AudioTool
{
    public class SoundManager : SoundManager<SoundType>
    {
        public SoundManager(IPrefabPool prefabPool, IRandomService randomService) : base(prefabPool, randomService, SoundType.Unspecified)
        {
        }

        protected override bool IsDefaultSoundType(SoundType soundType)
        {
            return base.IsDefaultSoundType(soundType) ||
                   soundType == SoundType.GenericMusic ||
                   soundType == SoundType.GenericSound;
        }
    }
}