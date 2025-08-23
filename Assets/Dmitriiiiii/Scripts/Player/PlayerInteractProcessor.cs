using UnityEngine;

namespace Dmi.Scripts.Player
{
    [System.Serializable]
    public class PlayerInteractProcessor
    {
        [Header("Buttons")] [SerializeField] PressButton _interactPressButton;
        [SerializeField] PressButton _interactLongPressButton;
        [SerializeField] PressButton _interactHoldButton;

        [Space] [SerializeField] PressButton _cancelButton;
        [SerializeField] PressButton _cancelLongPressButton;

        IInteractIndicator _indicator;
        IInteractable _currentInteractable;

        public void Initialize(IInteractIndicator indicator)
        {
            _indicator = indicator;

            _interactPressButton.SetPressAction(OnInteractPressed);
            _interactLongPressButton.SetPressAction(OnInteractPressed, onProgressAction: OnPressProgress);
            _cancelButton.SetPressAction(OnInteractPressed);
            _cancelLongPressButton.SetPressAction(OnInteractPressed, onProgressAction: OnPressProgress);

            _interactHoldButton.SetPressAction(OnHoldInteractStopped, OnHoldInteractStarted);
        }

        public void SetIInteractible(IInteractable interactable)
        {
            if (_currentInteractable == interactable)
                return;

            _indicator.SetPressProgress(0f);
            _currentInteractable?.OnStopHoldInteract();

            _currentInteractable = interactable;
            OnValuesUpdated(interactable);
        }

        private void OnInteractPressed()
        {
            _currentInteractable?.TryStartInteract(OnValuesUpdated);
            _indicator.SetPressProgress(0f);
        }

        private void OnPressProgress(float progress01)
        {
            _indicator.SetPressProgress(progress01);
        }

        private void OnHoldInteractStarted()
        {
            _currentInteractable?.OnStartHoldInteract();
        }

        private void OnHoldInteractStopped()
        {
            _currentInteractable?.OnStopHoldInteract();
        }

        private void OnValuesUpdated(IInteractable interactable)
        {
            if (_currentInteractable == null)
            {
                DisableAll();
                _indicator?.Hide();
                return;
            }

            if (_currentInteractable != interactable) return;

            _indicator.Show();
            _indicator.Initialize(_currentInteractable);

            bool isLongPress = _currentInteractable.IsLongPress;
            bool isInteract = _currentInteractable.IsButtonInteract;
            bool isHold = _currentInteractable.IsHold;

            DisableAll();

            if (isHold)
            {
                if (isInteract)
                    _interactHoldButton.EnableAction();
                else
                    _cancelButton.EnableAction();
                return;
            }

            if (isLongPress)
            {
                if (isInteract)
                    _interactLongPressButton.EnableAction();
                else
                    _cancelLongPressButton.EnableAction();
            }
            else
            {
                if (isInteract)
                    _interactPressButton.EnableAction();
                else
                    _cancelButton.EnableAction();
            }
        }

        private void DisableAll()
        {
            _interactPressButton.DisableAction();
            _interactLongPressButton.DisableAction();
            _interactHoldButton.DisableAction();

            _cancelButton.DisableAction();
            _cancelLongPressButton.DisableAction();
        }
    }
}
