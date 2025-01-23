using System;
using System.Threading;
using Additional;
using Cysharp.Threading.Tasks;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameObjects.CameraControllers.Workshop
{
    public class CameraTrackingTargetController : MonoBehaviour
    {
        [Required] [SerializeField] Transform _constructViewPoint;

        [HideLabel] [Required] [SerializeField]
        Transform[] _borders;

        [SerializeField] float _moveSpeed;

        Controls _controls;

        Func<Vector2> _inputValue;
        event Action SubscribeEvents;
        event Action DisposeEvents;

        CancellationTokenSource _moveCts;

        [Zenject.Inject]
        private void Construct(Controls controls)
        {
            SubscribeAllEvents(controls);
        }

        private void SubscribeAllEvents(Controls controls)
        {
            SubscribeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraMouseMovementMode.started += OnMouseMovementEnable;
            };
            SubscribeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraAnyMovement.started += OnAnyMovementEnable;
            };
            SubscribeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraMouseMovementMode.canceled += OnMovementStop;
            };
            SubscribeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraAnyMovement.canceled += OnMovementStop;
            };

            DisposeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraMouseMovementMode.started -= OnMouseMovementEnable;
            };
            DisposeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraAnyMovement.started -= OnAnyMovementEnable;
            };
            DisposeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraMouseMovementMode.canceled -= OnMovementStop;
            };
            DisposeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraAnyMovement.canceled -= OnMovementStop;
            };
        }

        private void OnEnable()
        {
            SubscribeEvents?.Invoke();
        }

        private void OnDisable()
        {
            DisposeEvents?.Invoke();
        }

        private void OnMouseMovementEnable(InputAction.CallbackContext context)
        {
            _inputValue = _controls.Workshop.CameraMouseMovement.ReadValue<Vector2>;
            OnMovementStart();
        }

        private void OnAnyMovementEnable(InputAction.CallbackContext context)
        {
            _inputValue = _controls.Workshop.CameraAnyMovement.ReadValue<Vector2>;
            OnMovementStart();
        }

        private void OnMovementStart()
        {
            if (_moveCts != null) return;

            _moveCts = new();
            MovementLoop(_moveCts.Token).Forget();
        }

        private void OnMovementStop(InputAction.CallbackContext context)
        {
            ClearToken();
        }

        private async UniTask MovementLoop(CancellationToken token)
        {
            while (true)
            {
                var inputValue = _inputValue?.Invoke(); //_controls.Workshop.CameraMovement.ReadValue<Vector2>();
                Debug.Log(inputValue);
                await UniTask.WaitForFixedUpdate(cancellationToken: token);
            }
        }

        private void ClearToken() => ClearTokenSupport.ClearToken(ref _moveCts);

        private void OnDestroy()
        {
            ClearToken();
            DisposeEvents?.Invoke();
        }

        private void Reset()
        {
            if (_borders.Length == 0)
            {
                _borders = new Transform[6];

                _borders[0] = new GameObject("CameraBorder(x)").transform;
                _borders[1] = new GameObject("CameraBorder(-x)").transform;
                _borders[2] = new GameObject("CameraBorder(y)").transform;
                _borders[3] = new GameObject("CameraBorder(-y)").transform;
                _borders[4] = new GameObject("CameraBorder(z)").transform;
                _borders[5] = new GameObject("CameraBorder(-z)").transform;
            }
        }
    }
}