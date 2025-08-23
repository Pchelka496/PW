using UnityEngine;
using Zenject;

namespace Dmi.Scripts.Player.DI
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] PlayerCore _playerCore;
        [SerializeField] PlayerSettings _playerSettings;


        public override void InstallBindings()
        {
            Container.Bind<PlayerCore>().FromInstance(_playerCore).NonLazy();
            Container.Bind<PlayerSettings>().FromInstance(_playerSettings).NonLazy();
        }
    }
}