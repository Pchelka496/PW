using R3;

namespace _Project.Scripts.Game.Audio
{
    public interface IAudioSettingContainer
    {
        public ReadOnlyReactiveProperty<bool> SoundEnabledFlag { get; }
        public ReadOnlyReactiveProperty<float> MaxVolume { get; }
        public ReadOnlyReactiveProperty<float> MusicVolume { get; }
    }
}