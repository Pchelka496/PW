using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Additional;
using Cysharp.Threading.Tasks;
using GameObjects.CameraControllers.Workshop;
using GameObjects.Command;
using GameObjects.Construct.Parts;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameObjects.Construct.Workshop
{
    [Serializable]
    public class ConstructPartPlacementController : IDisposable
    {
        const float ROTATION_VALUE = 90f;

        [SerializeField] LayerMask _placementLayerMask;
        ConstructPartPlacementComponent _constructPart;
        Construct _construct;
        Camera _mainCamera;
        bool _isPlacing;

        Func<Vector3> _getRotationInputValue;
        Func<Vector3> _getMousePosition;

        IDisposable _playerConstructSubscription;
        CancellationTokenSource _cts;

        public ReactiveProperty<bool> PlacementOpportunity { get; private set; }

        [Zenject.Inject]
        private void Construct(
            Controls controls,
            Camera mainCamera,
            WorkshopCameraController workshopCameraController,
            ConstructRegistry constructRegistry)
        {
            _mainCamera = mainCamera;
            _getMousePosition = () => Mouse.current.position.ReadValue();
            _getRotationInputValue = () => controls.Workshop.PartRotation.ReadValue<Vector3>();

            _playerConstructSubscription = constructRegistry.CurrentPlayerConstruct
                .Subscribe(newConstruct => _construct = newConstruct);

            _cts = new();
            PlacementLoop(_cts.Token).Forget();
        }

        public void StartPlacement(ConstructPartPlacementComponent constructPart)
        {
            _constructPart = constructPart;
            PlacementOpportunity = new ReactiveProperty<bool>(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PlacementOpportunityInspect()
        {
            if (_construct.MaxPartCount == 0)
            {
                PlacementOpportunity.Value = Mathf.Abs(_constructPart.transform.position.y) < 0.1f;
            }
            else
            {
                PlacementOpportunity.Value = _constructPart.CollisionCount.Value > 0;
            }
        }

        private async UniTaskVoid PlacementLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitForFixedUpdate();
                if (_constructPart == null) continue;

                _constructPart.transform.position = GetConstructPartPosition();
                RotateBuilding();
                PlacementOpportunityInspect();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RotateBuilding()
        {
            var rotationInput = _getRotationInputValue();

            if (rotationInput.sqrMagnitude > 0.01f)
            {
                var rotationAmount = rotationInput * ROTATION_VALUE;
                var currentRotation = _constructPart.transform.rotation.eulerAngles;

                _constructPart.transform.rotation = Quaternion.Euler(
                    x: currentRotation.x + rotationAmount.x,
                    y: currentRotation.y + rotationAmount.y,
                    z: currentRotation.z + rotationAmount.z);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 GetConstructPartPosition()
        {
            var ray = _mainCamera.ScreenPointToRay(_getMousePosition.Invoke());

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
            {
                var position = hit.point;

                if (_construct.MaxPartCount == 0)
                {
                    // Находим точку пересечения луча с плоскостью Y = 0
                    var t = (0 - ray.origin.y) /
                            ray.direction.y; // t — это параметр, определяющий точку пересечения луча с плоскостью

                    if (t >= 0)
                    {
                        position = ray.origin + ray.direction * t; // Получаем точку на луче, где y = 0
                    }
                }

                return position;
            }

            return Vector3.zero;
        }

        private void ClearToken(ref CancellationTokenSource cts) => ClearTokenSupport.ClearToken(ref cts);

        public void Dispose()
        {
            _playerConstructSubscription?.Dispose();
            ClearToken(ref _cts);
        }
    }
}