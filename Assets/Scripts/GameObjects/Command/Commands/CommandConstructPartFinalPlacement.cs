using GameObjects.Construct.Parts;
using UnityEngine;

namespace GameObjects.Command.Commands
{
    public readonly struct CommandConstructPartFinalPlacement : ICommand
    {
        readonly ConstructPartCore _constructPart;
        readonly Vector3 _placementPosition;

        public CommandConstructPartFinalPlacement(ConstructPartCore constructPart, Vector3 placementPosition)
        {
            _constructPart = constructPart;
            _placementPosition = placementPosition;
        }

        public bool IsCancelable => true;

        public void Execute()
        {
            _constructPart.transform.position = _placementPosition;
        }

        public void Undo()
        {
            
        }
    }
}