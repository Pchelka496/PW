using System;
using UnityEngine;
using Zenject;

namespace Dmi.Scripts.Core.DI
{
    public class CoreInstaller : MonoInstaller
    {
        [SerializeField] MainCameraController _cameraController;

        Controls _controls;

        public override void InstallBindings()
        {
            _controls = new();
            _controls.Enable();

            Container.Bind<Controls>().FromInstance(_controls).NonLazy();
            Container.Bind<MainCameraController>().FromInstance(_cameraController).NonLazy();

            Container.Bind(typeof(IDisposable), typeof(DistributedUpdateLoop)).To<DistributedUpdateLoop>().AsSingle()
                .NonLazy();

            Container.Bind(typeof(IDisposable),typeof(ITickable), typeof(Timer)).To<Timer>().AsSingle().NonLazy();
        }

        private void OnDestroy()
        {
            _controls?.Dispose();
        }
    }
}