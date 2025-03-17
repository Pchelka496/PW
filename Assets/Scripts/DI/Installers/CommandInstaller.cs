using System;
using GameObjects.CameraControllers.Workshop;
using Zenject;
using GameObjects.Command.Commands;
using GameObjects.Construct.Workshop;
using UnityEngine;

namespace DI.Installers
{
    public class CommandInstaller : MonoInstaller
    {
        ConstructPartPlacementController _constructPartPlacementController;

        public override void InstallBindings()
        {
            _constructPartPlacementController = new();

            CommandConstructPartStartPlacement.Initialize(_constructPartPlacementController);
            //CommandConstructPartFinalPlacement
        }

        private void Awake()
        {
            Container.Inject(_constructPartPlacementController);
        }

        private void OnDestroy()
        {
            _constructPartPlacementController?.Dispose();
        }
    }
}