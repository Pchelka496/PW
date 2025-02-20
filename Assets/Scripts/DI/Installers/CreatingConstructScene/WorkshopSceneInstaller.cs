using GameObjects.Construct;
using Zenject;

namespace DI.Installers.CreatingConstructScene
{
    public class WorkshopSceneInstaller : MonoInstaller
    {
        Controls _controls;
        
        public override void InstallBindings()
        {
            InitializeControls(out _controls);
            Container.Bind<Controls>().FromInstance(_controls).AsSingle().NonLazy();
            Container.Bind<ConstructPartsDataLoader>().FromNew().AsSingle().NonLazy();
            Container.Bind<ConstructPartFactory>().FromNew().AsSingle().Lazy();
        }
        
        private void InitializeControls(out Controls controls)
        {
            controls = new Controls();
            controls.Enable();
        }
        
        private void OnDestroy()
        {
            _controls.Disable();
        }
    }
}