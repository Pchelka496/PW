using System;
using GameObjects.CameraControllers.Workshop;
using GameObjects.Construct;
using GameObjects.Construct.Workshop;
using GameObjects.UI.Workshop.ConstructPartsShop;
using GameObjects.UI.Workshop.ConstructPartsShop.TextureCreators;
using UnityEngine;
using Zenject;

namespace DI.Installers.CreatingConstructScene
{
    public class WorkshopSceneInstaller : MonoInstaller, ConstructPartsShopPage.IConstructPartsShopPageInstaller
    {
        [SerializeField] Transform _borderParentTransform;
        [SerializeField] ConstructPartTextureManager _constructPartTextureManager;
        [SerializeField] Camera _mainCamera;
        [SerializeField] WorkshopCameraController _workshopCameraController;
        readonly CameraBordersColliders _borderCreator = new(); 
        readonly ConstructRegistry _constructRegistry = new();
        
        Controls _controls;

        public DiContainer DiContainer => Container;

        public override void InstallBindings()
        {
            InitializeControls(out _controls);

            Container.Bind<ConstructPartsShopPage.IConstructPartsShopPageInstaller>().FromInstance(this).AsSingle()
                .NonLazy();
            Container.Bind<ConstructPartTextureManager>().FromInstance(_constructPartTextureManager).AsSingle().Lazy();
            Container.Bind<Controls>().FromInstance(_controls).AsSingle().NonLazy();
            Container.Bind<ConstructPartsDataLoader>().FromNew().AsSingle().NonLazy();
            Container.Bind<ConstructPartFactory>().FromNew().AsSingle().Lazy();
            Container.Bind<Camera>().FromInstance(_mainCamera).AsSingle().NonLazy();
            Container.Bind<WorkshopCameraController>().FromInstance(_workshopCameraController).AsSingle().NonLazy();
            Container.Bind<ConstructRegistry>().FromInstance(_constructRegistry).AsSingle().NonLazy();
        }

        private void InitializeControls(out Controls controls)
        {
            controls = new Controls();
            controls.Enable();
        }

        private void Awake()
        {
            CreateConstructPartBorder();
        }

        public void CreateConstructPartBorder()
        {
            const float OFFSET = 20f;
            var cameraBorder = _workshopCameraController.MovementController.CameraBorder;
            var constructPartMaxPosition = new Vector3();

            constructPartMaxPosition.x =
                Mathf.Max(cameraBorder.LeftBorder.position.x, cameraBorder.RightBorder.position.x) + OFFSET;

            constructPartMaxPosition.y =
                Mathf.Max(cameraBorder.UpBorder.position.y, cameraBorder.DownBorder.position.y) + OFFSET;

            constructPartMaxPosition.z =
                Mathf.Max(cameraBorder.FrontBorder.position.z, cameraBorder.BackBorder.position.z) + OFFSET;

            _borderCreator.Initialize(_borderParentTransform, Vector3.zero, constructPartMaxPosition)
                .CreateColliders();
        }

        private void OnDestroy()
        {
            _controls.Disable();
        }
    }
}