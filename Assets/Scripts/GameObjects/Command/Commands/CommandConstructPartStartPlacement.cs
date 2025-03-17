using GameObjects.Construct.Parts;
using GameObjects.Construct.Workshop;
using UnityEngine;

namespace GameObjects.Command.Commands
{
    public readonly struct CommandConstructPartStartPlacement : ICommand
    {
        static ConstructPartPlacementController _placementController;
        readonly ConstructPartPlacementComponent _constructPart;

        public static void Initialize(ConstructPartPlacementController placementController)
        {
            _placementController = placementController;
        }

        public CommandConstructPartStartPlacement(ConstructPartPlacementComponent constructPart)
        {
            _constructPart = constructPart;
        }

        public bool IsCancelable => true;

        public void Execute()
        {
            _placementController.StartPlacement(_constructPart);
            _constructPart.StartPlacement(_placementController);
        }

        public void Undo()
        {
            if (_constructPart == null) return;
            Object.Destroy(_constructPart.gameObject);
        }
    }
}