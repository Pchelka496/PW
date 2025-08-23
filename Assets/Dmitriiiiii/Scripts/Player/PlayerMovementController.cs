using System;
using System.Runtime.CompilerServices;
using _Project.Scripts.Game.Audio;
using Cysharp.Threading.Tasks;
using Dmi.Scripts.Audio;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Dmi.Scripts.Player
{
    [Serializable]
    public class PlayerMovementController : IPlayerComponent, ITickable, IDisposable, IDistributedUpdatable,
        IFixedTickable
    {
        [SerializeField] Transform _playerTransform;
        [SerializeField] GroundChecker _groundChecker;
         Characteristics _characteristics;
        [SerializeField] CharacterController _controller;
        [Space] [SerializeField] EnumUpdateFrequency _rayCastFrequency;
        [Space] [SerializeField] AudioContainer _jump;
        [SerializeField] AudioContainer _landing;
        [Space] [SerializeField] AudioContainer _walkingAsphalt;
        [SerializeField] AudioContainer _walkingEarth;
        [SerializeField] AudioContainer _walkingWood;
        [Space] [SerializeField] float _walkAudioRate;
        [SerializeField] float _runAudioRate;
        [SerializeField] Transform _rayCastStartPoint;
        [SerializeField] float _stepRaycastDistance = 1.5f;
        [SerializeField] LayerMask _stepLayerMask;
        [SerializeField] string _asphaltTag = "Asphalt";
        [SerializeField] string _earthTag = "Earth";
        [SerializeField] string _woodTag = "Wood";

        [Header("Camera Bobbing")] [SerializeField]
        Transform _cameraTarget;

        [SerializeField] float _bobAmplitudeWalk = 0.05f;
        [SerializeField] float _bobFrequencyWalk = 7f;
        [SerializeField] float _bobAmplitudeSprint = 0.1f;
        [SerializeField] float _bobFrequencySprint = 10f;
        [SerializeField] float _bobSmoothSpeed = 10f;
        [SerializeField] Vector3 _initialCameraLocalPos;

        float _bobTime;

        readonly RaycastHit[] _raycastHits = new RaycastHit[10];

        DistributedUpdateLoop _distributedUpdateLoop;
        Controls _controls;
        FirstPersonCameraController _cameraController;

        string _currentSurfaceTag;
        float _stepTimer;
        AudioContainer _currentSurfaceSound;

        int _movementBlockCounter;
        bool _isMovementActive;

        float _currentMoveSpeed;
        float _verticalVelocity;

        bool _isGrounded;
        bool _isSprinting;
        bool _useGravity;

        float _targetMoveSpeed;

        IDisposable _checkGroundDisposable;
        Func<Vector2> _getMovementInput = () => Vector2.zero;

        float _lastJumpTime = float.MinValue;

        public bool UseGravity
        {
            get => _useGravity;
            set => _useGravity = value;
        }

        [Inject]
        private void Construct(Controls controls, PlayerSettings playerSettings, PlayerCore core,
            DistributedUpdateLoop updateLoop, Timer timer)
        {
            _distributedUpdateLoop = updateLoop;
            _controls = controls;
            _cameraController = core.FirstPersonCameraController;
            _characteristics = playerSettings.MovementCharacteristics;
            _currentMoveSpeed = _characteristics.WalkSpeed;

            _currentMoveSpeed = _targetMoveSpeed = _characteristics.WalkSpeed;

            _checkGroundDisposable = _groundChecker.IsGrounded.Subscribe(OnGroundStatusChanged);

            _distributedUpdateLoop.Register(_rayCastFrequency, this);
            EnableMovement();
        }

        private void OnGroundStatusChanged(bool isGrounded)
        {
            try
            {
                _isGrounded = isGrounded;
                if (_isGrounded)
                {
                    _landing.PlayInOrder();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void EnableMovement()
        {
            _movementBlockCounter = Mathf.Max(0, _movementBlockCounter - 1);

            if (_movementBlockCounter == 0 && !_isMovementActive)
            {
                _getMovementInput = _controls.Player.Move.ReadValue<Vector2>;
                _controls.Player.Jump.performed += Jump;

                _controls.Player.Sprint.performed += OnSprintToggle;

                _isMovementActive = true;
            }
        }

        public void DisableMovement()
        {
            _movementBlockCounter++;

            if (_isMovementActive)
            {
                _getMovementInput = () => Vector2.zero;
                _controls.Player.Jump.performed -= Jump;

                _controls.Player.Sprint.performed -= OnSprintToggle;

                _isMovementActive = false;
            }
        }

        private void OnSprintToggle(InputAction.CallbackContext ctx)
        {
            SetSprint(!_isSprinting);
        }

        public async UniTaskVoid SetPlayerPosition(Vector3 newPosition)
        {
            _controller.enabled = false;
            await UniTask.Yield();
            _playerTransform.position = newPosition;
            await UniTask.Yield();
            _controller.enabled = true;
        }

        public void FixedTick()
        {
            if (!_useGravity)
            {
                _verticalVelocity = 0f;
                return;
            }

            if (!_isGrounded) ApplyGravity();
        }

        public void Tick()
        {
            PlayFootstepSound();
            Move();
            UpdateCameraBobbing();
        }

        private void UpdateCameraBobbing()
        {
            if (!_isMovementActive) return;

            if (!_isMovementActive || !_isGrounded)
            {
                _bobTime = 0f;
                _cameraTarget.localPosition = Vector3.Lerp(_cameraTarget.localPosition, _initialCameraLocalPos,
                    Time.deltaTime * _bobSmoothSpeed);
                return;
            }

            Vector2 input = _getMovementInput();
            if (input == Vector2.zero)
            {
                _bobTime = 0f;
                _cameraTarget.localPosition = Vector3.Lerp(_cameraTarget.localPosition, _initialCameraLocalPos,
                    Time.deltaTime * _bobSmoothSpeed);
                return;
            }

            float speedMultiplier = _isSprinting ? _bobFrequencySprint : _bobFrequencyWalk;
            float amplitude = _isSprinting ? _bobAmplitudeSprint : _bobAmplitudeWalk;

            _bobTime += Time.deltaTime * speedMultiplier;

            float bobOffsetY = Mathf.Sin(_bobTime * Mathf.PI * 2f) * amplitude;

            Vector3 targetPos = _initialCameraLocalPos + new Vector3(0f, bobOffsetY, 0f);
            _cameraTarget.localPosition =
                Vector3.Lerp(_cameraTarget.localPosition, targetPos, Time.deltaTime * _bobSmoothSpeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Move()
        {
            var movementInput = _getMovementInput();
            if (movementInput == Vector2.zero) SetSprint(false);

            float accelerationFactor = _currentMoveSpeed < 0.01f &&
                                       Mathf.Approximately(_targetMoveSpeed, _characteristics.WalkSpeed)
                ? _characteristics.AccelerationTime * 0.5f
                : (_isSprinting ? _characteristics.AccelerationTime : _characteristics.DecelerationTime);

            _currentMoveSpeed = Mathf.Lerp(_currentMoveSpeed, _targetMoveSpeed, Time.deltaTime / accelerationFactor);


            var cameraTransform = _cameraController.GetCameraTransform;

            var forward = cameraTransform.forward;
            forward.y = 0;
            forward.Normalize();

            var right = cameraTransform.right;
            right.y = 0;
            right.Normalize();

            var moveDirection = right * movementInput.x + forward * movementInput.y;

            _controller.Move(moveDirection.normalized * (_currentMoveSpeed * Time.deltaTime) +
                             Vector3.up * _verticalVelocity * Time.deltaTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PlayFootstepSound()
        {
            if (!_isMovementActive || !_isGrounded || _currentSurfaceSound == null)
                return;

            var input = _getMovementInput();
            if (input == Vector2.zero)
                return;

            _stepTimer -= Time.deltaTime;

            if (_stepTimer <= 0)
            {
                _currentSurfaceSound.PlayInOrder();
                _stepTimer = _isSprinting ? _runAudioRate : _walkAudioRate;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyGravity()
        {
            _verticalVelocity += _characteristics.Gravity * Time.deltaTime;
        }

        private void Jump(InputAction.CallbackContext ctx)
        {
            if (!_isGrounded) return;

            float timeSinceLastJump = Time.time - _lastJumpTime;
            if (timeSinceLastJump < _characteristics.JumpCooldown)
                return;

            _lastJumpTime = Time.time;
            _verticalVelocity = Mathf.Sqrt(_characteristics.JumpHeight * -2f * _characteristics.Gravity);
            _jump.PlayInOrder();
        }

        private void OnSpringButtonPerformed(InputAction.CallbackContext ctx) => SetSprint(true);
        private void OnSpringButtonCanceled(InputAction.CallbackContext ctx) => SetSprint(false);

        private void SetSprint(bool isSprinting)
        {
            _isSprinting = isSprinting;
            _targetMoveSpeed = _isSprinting ? _characteristics.SprintSpeed : _characteristics.WalkSpeed;
        }

        public void DistributedUpdate()
        {
            if (!_isGrounded) return;

            int hitCount = Physics.RaycastNonAlloc(
                _rayCastStartPoint.position,
                Vector3.down,
                _raycastHits,
                _stepRaycastDistance,
                _stepLayerMask
            );

            if (hitCount <= 0) return;

            float minDistance = float.MaxValue;
            RaycastHit nearestHit = default;

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _raycastHits[i];
                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    nearestHit = hit;
                }
            }

            _currentSurfaceTag = nearestHit.collider.tag;

#if UNITY_EDITOR
            Debug.DrawRay(_rayCastStartPoint.position, Vector3.down * _stepRaycastDistance, Color.red, 0.1f);
#endif

            _currentSurfaceSound = _currentSurfaceTag switch
            {
                var t when t == _asphaltTag => _walkingAsphalt,
                var t when t == _earthTag => _walkingEarth,
                var t when t == _woodTag => _walkingWood,
                _ => null
            };
        }

        public void Dispose()
        {
            _distributedUpdateLoop.Unregister(_rayCastFrequency, this);
            _checkGroundDisposable?.Dispose();
        }

        [Serializable]
        public struct Characteristics
        {
            [SerializeField] public float WalkSpeed;
            [SerializeField] public float SprintSpeed;
            [SerializeField] public float AccelerationTime;
            [SerializeField] public float DecelerationTime;
            [SerializeField] public float JumpHeight;
            [SerializeField] public float Gravity;
            [SerializeField] public float JumpCooldown; // Новое поле
            [SerializeField] public AnimationCurve AccelerationEase;
            [SerializeField] public AnimationCurve DecelerationEase;
        }
    }
}
