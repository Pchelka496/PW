using UnityEngine;
using Zenject;

namespace DI.Installers.CreatingConstructScene
{
    public class WorkshopSceneInstaller : MonoInstaller
    {
        Controls _controls;

        public override void InstallBindings()
        {
            InitializeControls(out _controls);
            Container.BindInterfacesAndSelfTo<Controls>().FromInstance(_controls).AsSingle().NonLazy();
        }

        private void InitializeControls(out Controls controls)
        {
            controls = new Controls();
            controls.Enable();
        }

        private void OnDestroy()
        {
            _controls.Disable();
            _controls.Dispose();
        }
    }
}