
using Dmi.Scripts.Player;

namespace Dmi.Scripts
{
    public interface IInteractIndicator
    {
        void Hide();
        void Show();

        void Initialize(IInteractable interactable);
        void SetPressProgress(float progress01);
    }
}
