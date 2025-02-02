using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Additional;
using Cysharp.Threading.Tasks;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace GameObjects.CameraControllers.Workshop
{
    public class CameraMovementController : MonoBehaviour
    {
        const float MOUSE_INPUT_VALUE_LIMIT = 1f;

        [HideLabel] [Required] [SerializeField]
        Camera _camera;

        [HideLabel] [Required] [SerializeField]
        CinemachineOrbitalFollow _orbitalFollow;

        [Required] [SerializeField] Transform _constructViewPoint;
        [SerializeField] CameraBorders _borders;

        [Header("Camera speed Settings")] [Range(0f, float.MaxValue)] [SerializeField]
        float _maxMoveSpeed = 20f;
        
        [Range(0f, float.MaxValue)] [SerializeField]
        float _mouseControlsMaxMoveSpeed = 40f;
        
        [Range(0f, float.MaxValue)] [SerializeField]
        float _speedNearBorders = 10f;
        
        [Range(0f, float.MaxValue)] [SerializeField]
        float _minDistanceToReduceSpeed = 3f;

        bool _isMovingPossibility;
        Controls _controls;
        float _cameraMoveRadius;

        private Func<float> _getMaxMoveSpeed;
        Func<Vector3> _inputValue;
        event Action SubscribeEvents;
        event Action DisposeEvents;

        CancellationTokenSource _moveCts;

#if UNITY_EDITOR
        [field: SerializeField] public bool VisualizeBorders { get; private set; }
        public CameraBorders CameraBorder => _borders;
        public float CameraMoveRadius => _orbitalFollow.Radius;
        public float MinDistanceToReduceSpeed => _minDistanceToReduceSpeed;
#endif

        [Inject]
        private void Construct(Controls controls)
        {
            _controls = controls;

            SubscribeAllEvents(controls);
        }

        private void SubscribeAllEvents(Controls controls)
        {
            SubscribeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraMouseMovementMode.started += OnMouseMovementEnable;
            };
            DisposeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraMouseMovementMode.started -= OnMouseMovementEnable;
            };

            SubscribeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraAnyMovement.started += OnAnyMovementEnable;
            };
            DisposeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraAnyMovement.started -= OnAnyMovementEnable;
            };

            SubscribeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraAnyMovement.canceled += OnMovementDisable;
            };
            DisposeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraAnyMovement.canceled -= OnMovementDisable;
            };

            SubscribeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraMouseMovementMode.canceled += OnMovementDisable;
            };
            DisposeEvents += () =>
            {
                if (controls == null) return;
                controls.Workshop.CameraMouseMovementMode.canceled -= OnMovementDisable;
            };
        }

        private void Awake()
        {
            _cameraMoveRadius = _orbitalFollow.Radius;
        }

        private void OnEnable()
        {
            OnMovementStart();
            SubscribeEvents?.Invoke();
        }

        private void OnDisable()
        {
            OnMovementStop();
            DisposeEvents?.Invoke();
        }

        private void OnMouseMovementEnable(InputAction.CallbackContext context)
        {
            _inputValue = () =>
            {
                var inputValue = _controls.Workshop.CameraMouseMovement.ReadValue<Vector3>();

                return new Vector3(
                    Mathf.Clamp(inputValue.x, -MOUSE_INPUT_VALUE_LIMIT, MOUSE_INPUT_VALUE_LIMIT),
                    Mathf.Clamp(inputValue.y, -MOUSE_INPUT_VALUE_LIMIT, MOUSE_INPUT_VALUE_LIMIT),
                    Mathf.Clamp(inputValue.z, -MOUSE_INPUT_VALUE_LIMIT, MOUSE_INPUT_VALUE_LIMIT)
                );
            };

            _getMaxMoveSpeed = () => _mouseControlsMaxMoveSpeed;
            _isMovingPossibility = true;
        }

        private void OnAnyMovementEnable(InputAction.CallbackContext context)
        {
            _inputValue = _controls.Workshop.CameraAnyMovement.ReadValue<Vector3>;
            _getMaxMoveSpeed = () => _maxMoveSpeed;
            _isMovingPossibility = true;
        }

        private void OnMovementDisable(InputAction.CallbackContext context)
        {
            _isMovingPossibility = false;
        }

        private void OnMovementStart()
        {
            if (_moveCts != null) return;

            _moveCts = new();
            MovementLoop(_moveCts.Token).Forget();
        }

        private void OnMovementStop()
        {
            ClearToken();
        }

        private async UniTask MovementLoop(CancellationToken token)
        {
            while (true)
            {
                var inputValue = _inputValue?.Invoke();

                if (_isMovingPossibility & inputValue.HasValue)
                {
                    var deltaTime = Time.deltaTime;

                    var forward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
                    var right = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;
                    var up = Vector3.up;

                    var moveDirection =
                        right * inputValue.Value.x +
                        forward * inputValue.Value.z +
                        up * inputValue.Value.y;

                    var moveSpeed = GetMoveSpeed(_constructViewPoint.position, moveDirection);

                    var moveDelta = new Vector3(
                        x: moveDirection.x * moveSpeed.x * deltaTime,
                        y: moveDirection.y * moveSpeed.y * deltaTime,
                        z: moveDirection.z * moveSpeed.z * deltaTime);

                    var newPosition = _constructViewPoint.position + moveDelta;
                    //ClampPositionWithinBorders(ref newPosition);

                    _constructViewPoint.position = newPosition;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 GetMoveSpeed(Vector3 position, Vector3 direction)
        {
            var (minPositions, maxPositions) = GetLimitPositions();

            var minXFactor = Mathf.Clamp01((position.x - minPositions.x) / _minDistanceToReduceSpeed);
            var maxXFactor = Mathf.Clamp01((maxPositions.x - position.x) / _minDistanceToReduceSpeed);

            var minYFactor = Mathf.Clamp01((position.y - minPositions.y) / _minDistanceToReduceSpeed);
            var maxYFactor = Mathf.Clamp01((maxPositions.y - position.y) / _minDistanceToReduceSpeed);

            var minZFactor = Mathf.Clamp01((position.z - minPositions.z) / _minDistanceToReduceSpeed);
            var maxZFactor = Mathf.Clamp01((maxPositions.z - position.z) / _minDistanceToReduceSpeed);

            return new(
                x: GetMoveSpeed(minXFactor, maxXFactor, direction.x),
                y: GetMoveSpeed(minYFactor, maxYFactor, direction.y),
                z: GetMoveSpeed(minZFactor, maxZFactor, direction.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetMoveSpeed(float minFactor, float maxFactor, float direction)
        {
            if (minFactor < 1f && direction < 0)
            {
                return _speedNearBorders * minFactor;
            }

            if (maxFactor < 1f && direction > 0)
            {
                return _speedNearBorders * maxFactor;
            }

            return _getMaxMoveSpeed.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (Vector3 minPositions, Vector3 maxPositions) GetLimitPositions()
        {
            var minX = _borders.LeftBorder.position.x + _cameraMoveRadius;
            var maxX = _borders.RightBorder.position.x - _cameraMoveRadius;

            var minY = _borders.DownBorder.position.y + _cameraMoveRadius;
            var maxY = _borders.UpBorder.position.y - _cameraMoveRadius;

            var minZ = _borders.BackBorder.position.z + _cameraMoveRadius;
            var maxZ = _borders.FrontBorder.position.z - _cameraMoveRadius;

            return (new(minX, minY, minZ), new(maxX, maxY, maxZ));
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // private void ClampPositionWithinBorders(ref Vector3 position)
        // {
        //     var (minPositions, maxPositions) = GetLimitPositions();
        //
        //     position.x = Mathf.Clamp(position.x, minPositions.x, maxPositions.x);
        //     position.y = Mathf.Clamp(position.y, minPositions.y, maxPositions.y);
        //     position.z = Mathf.Clamp(position.z, minPositions.z, maxPositions.z);
        // }

        private void ClearToken() => ClearTokenSupport.ClearToken(ref _moveCts);

        private void OnDestroy()
        {
            ClearToken();
            DisposeEvents?.Invoke();
        }

        private void Reset()
        {
            _camera = gameObject.GetComponent<Camera>();
            _orbitalFollow = gameObject.GetComponent<CinemachineOrbitalFollow>();

            _borders.UpBorder = new GameObject("CameraUpBorder").transform;
            _borders.DownBorder = new GameObject("CameraDownBorder").transform;

            _borders.LeftBorder = new GameObject("CameraLeftBorder").transform;
            _borders.RightBorder = new GameObject("CameraRightBorder").transform;

            _borders.FrontBorder = new GameObject("CameraFrontBorder").transform;
            _borders.BackBorder = new GameObject("CameraBackBorder").transform;
        }

        [Serializable]
        public record CameraBorders
        {
            [Required] public Transform UpBorder;
            [Required] public Transform DownBorder;

            [Required] public Transform LeftBorder;
            [Required] public Transform RightBorder;

            [Required] public Transform FrontBorder;
            [Required] public Transform BackBorder;
        }
    }
}