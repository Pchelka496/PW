using System;

namespace Dmi.Scripts.Player
{
    public interface IInteractable
    {
        bool IsButtonInteract { get; }
        bool IsLongPress { get; }
        bool IsHold { get;}
        bool TryGetInteractActionName(ref string interactActionName);
        void TryStartInteract(Action<IInteractable> onValueUpdated);

        void OnStartHoldInteract();
        void OnStopHoldInteract();
    }
}