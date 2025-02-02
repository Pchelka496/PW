using System;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameObjects.CameraControllers.Workshop
{
    public class OrbitalFollowController : MonoBehaviour
    {
        [HideLabel] [Required] [SerializeField]
        CinemachineOrbitalFollow _orbitalFollow;

        [HideLabel] [Required] [SerializeField]
        CinemachineInputAxisController _inputAxisController;

        InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller _lookOrbitX;
        InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller _lookOrbitY;
        InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller _orbitScale;

        bool _viewMode;
        CamaraMovingController _camaraMovingController = CamaraMovingController.NonControl;
        event Action SubscribeAllEvents;
        event Action DisposeEvents;

        [Zenject.Inject]
        public void Construct(Controls controls)
        {
            InitializeEvents(controls);
        }

        private void InitializeEvents(Controls controls)
        {
            SubscribeAllEvents += () =>
                controls.Workshop.ActiveLookMode.started += OnLookModeEnable;
            DisposeEvents += () =>
                controls.Workshop.ActiveLookMode.started -= OnLookModeEnable;

            SubscribeAllEvents += () =>
                controls.Workshop.ActiveLookMode.canceled += OnLookModeDisabled;
            DisposeEvents += () =>
                controls.Workshop.ActiveLookMode.canceled -= OnLookModeDisabled;

            SubscribeAllEvents += () =>
                controls.Workshop.CameraMouseMovementMode.started += OnCameraMovementMouseMovementStarted;
            DisposeEvents += () =>
                controls.Workshop.CameraMouseMovementMode.started -= OnCameraMovementMouseMovementStarted;

            SubscribeAllEvents += () =>
                controls.Workshop.CameraMouseMovementMode.canceled += OnCameraMouseMovementEnd;
            DisposeEvents += () =>
                controls.Workshop.CameraMouseMovementMode.canceled -= OnCameraMouseMovementEnd;
        }

        private void Awake()
        {
            InitializeOrbitalControllers();
        }

        private void InitializeOrbitalControllers()
        {
            foreach (var controller in _inputAxisController.Controllers)
            {
                switch (controller.Name)
                {
                    case "Look Orbit X":
                    {
                        _lookOrbitX = controller;
                        break;
                    }
                    case "Look Orbit Y":
                    {
                        _lookOrbitY = controller;
                        break;
                    }
                    case "Orbit Scale":
                    {
                        _orbitScale = controller;
                        break;
                    }
                }
            }

            _lookOrbitX.Enabled = false;
            _lookOrbitY.Enabled = false;
        }

        private void OnEnable() => SubscribeAllEvents?.Invoke();
        private void OnDisable() => DisposeEvents?.Invoke();

        private void OnLookModeEnable(InputAction.CallbackContext contex)
        {
            _viewMode = true;
            switch (_camaraMovingController)
            {
                case CamaraMovingController.Mouse:
                    return;
            }

            _lookOrbitX.Enabled = true;
            _lookOrbitY.Enabled = true;
        }

        private void OnLookModeDisabled(InputAction.CallbackContext contex)
        {
            _viewMode = false;

            _lookOrbitX.Enabled = false;
            _lookOrbitY.Enabled = false;
        }

        private void OnCameraMovementMouseMovementStarted(InputAction.CallbackContext contex)
        {
            _camaraMovingController = CamaraMovingController.Mouse;
            CursorManager.LockCursor();
            
            OnLookModeDisabled(contex);
        }

        private void OnCameraMouseMovementEnd(InputAction.CallbackContext contex)
        {
            _camaraMovingController = CamaraMovingController.NonControl;
            CursorManager.UnlockCursor();
            
            if (_viewMode)
            {
                OnLookModeEnable(contex);
            }
        }

        private void Reset()
        {
            gameObject.TryGetComponent<CinemachineOrbitalFollow>(out _orbitalFollow);
            gameObject.TryGetComponent<CinemachineInputAxisController>(out _inputAxisController);
        }

        private enum CamaraMovingController
        {
            Mouse,
            NonControl
        }
    }
}