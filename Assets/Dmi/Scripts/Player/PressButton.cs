using System;
using Dmi.Scripts;
using Dmi.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressButton : MonoBehaviour
{
    [Header("UI")] [SerializeField] ProgressIndicator _progressIndicator;
    [Header("Settings")] [SerializeField] ButtonAction _buttonAction;
    [SerializeField] float _pressDuration = 1f;
    [SerializeField] float _cooldownDuration = 0.2f;
    [SerializeField] bool _isHoldMode = false;

    static Controls _controls;
    static Timer _timer;

    int _currentTimerId = -1;

    bool _isCooldownActive;
    bool _wasPressed;
    bool _isPressInProgress;

    Action _completePressAction;
    Action _startPressAction;
    Action<float> _onProgressAction;

    [Zenject.Inject]
    private void Construct(Controls controls, Timer timer)
    {
        _controls = controls;
        _timer = timer;
    }

    private void Start()
    {
        if (!_isHoldMode)
            _progressIndicator?.SetProgress(0);
    }

    public PressButton SetPressAction(Action completePressAction, Action startPressAction = null,
        Action<float> onProgressAction = null)
    {
        _completePressAction = completePressAction;
        _startPressAction = startPressAction;
        _onProgressAction = onProgressAction;
        return this;
    }

    public void ClearProgress()
    {
        if (!_isHoldMode)
            _progressIndicator?.SetProgress(0);
    }

    public void ResetState()
    {
        _wasPressed = false;
        _isPressInProgress = false;
        _isCooldownActive = false;

        if (_currentTimerId != -1)
        {
            _timer.RemoveTimer(_currentTimerId);
            _currentTimerId = -1;
        }

        _progressIndicator?.SetProgress(0);
    }

    public void EnableAction()
    {
        ResetState();

        switch (_buttonAction)
        {
            case ButtonAction.Interact:
                _controls.Player.Interact.performed += StartPress;
                _controls.Player.Interact.canceled += CancelPress;
                break;
            case ButtonAction.Close:
                _controls.Player.Close.performed += StartPress;
                _controls.Player.Close.canceled += CancelPress;
                break;
        }
    }

    public void DisableAction()
    {
        if (_isHoldMode && _wasPressed)
            CancelPress(default);

        switch (_buttonAction)
        {
            case ButtonAction.Interact:
                _controls.Player.Interact.performed -= StartPress;
                _controls.Player.Interact.canceled -= CancelPress;
                break;
            case ButtonAction.Close:
                _controls.Player.Close.performed -= StartPress;
                _controls.Player.Close.canceled -= CancelPress;
                break;
        }
    }

    private void StartPress(InputAction.CallbackContext ctx)
    {
        if (_isCooldownActive || _isPressInProgress)
            return;

        _wasPressed = true;
        _isPressInProgress = true;
        _startPressAction?.Invoke();
        _progressIndicator?.SetProgress(0);

        if (!_isHoldMode && _pressDuration <= 0f)
        {
            CompletePress();
            return;
        }

        if (_isHoldMode) return;

        _currentTimerId = _timer.AddTimer(
            _pressDuration,
            CompletePress,
            progress =>
            {
                _progressIndicator?.SetProgress(progress);
                _onProgressAction?.Invoke(progress);
            }, unscaledTime: true);
    }

    private void CancelPress(InputAction.CallbackContext ctx)
    {
        if (!_isPressInProgress)
            return;

        _wasPressed = false;
        _isPressInProgress = false;

        if (_isHoldMode)
        {
            _completePressAction?.Invoke();
            StartCooldown();
            return;
        }

        if (_currentTimerId != -1)
        {
            _timer.RemoveTimer(_currentTimerId);
            _progressIndicator?.SetProgress(0);
            _onProgressAction?.Invoke(0f);
            _currentTimerId = -1;
        }
    }

    private void CompletePress()
    {
        _wasPressed = false;
        _progressIndicator?.SetProgress(1);
        _currentTimerId = -1;
        _completePressAction?.Invoke();
        StartCooldown();
    }

    private void StartCooldown()
    {
        if (_cooldownDuration <= 0f)
            return;

        _isCooldownActive = true;
        _timer.AddTimer(_cooldownDuration, () => _isCooldownActive = false, unscaledTime: true);
    }

    private void OnDestroy()
    {
        DisableAction();
    }

    private enum ButtonAction
    {
        Interact,
        Close,
    }
}