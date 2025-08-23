using _Project.Scripts.Game.Audio;
using UnityEngine;
using Zenject;

namespace Dmi.Scripts.Audio.DI
{
    public class AudioInstaller : MonoInstaller
    {
        [SerializeField] Transform _audioSourceParent;
        [SerializeField] UIAudioModule _uiAudioModule;

        AudioSourcePool _audioSourcePool;

        public override void InstallBindings()
        {
            _audioSourcePool = new(_audioSourceParent);

            Container.Bind<AudioSourcePool>().FromInstance(_audioSourcePool).NonLazy();
            Container.Bind<UIAudioModule>().FromInstance(_uiAudioModule).NonLazy();
        }

        private void Awake()
        {
            Container.Inject(_audioSourcePool);
        }

        private void OnDestroy()
        {
            _audioSourcePool?.Dispose();
        }
    }
}