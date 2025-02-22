using System;
using GameObjects.Construct;
using GameObjects.UI.Workshop.ConstructPartsShop;
using GameObjects.UI.Workshop.ConstructPartsShop.TextureCreators;
using UnityEngine;
using Zenject;

namespace DI.Installers.CreatingConstructScene
{
    public class WorkshopSceneInstaller : MonoInstaller, ConstructPartsShopPage.IConstructPartsShopPageInstaller
    {
        [SerializeField] ConstructPartTextureManager _constructPartTextureManager;
        Controls _controls;

        public DiContainer DiContainer => Container;

        public override void InstallBindings()
        {
            InitializeControls(out _controls);

            Container.Bind<ConstructPartsShopPage.IConstructPartsShopPageInstaller>().FromInstance(this).AsSingle().NonLazy();
            Container.Bind<ConstructPartTextureManager>().FromInstance(_constructPartTextureManager).AsSingle().Lazy();
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