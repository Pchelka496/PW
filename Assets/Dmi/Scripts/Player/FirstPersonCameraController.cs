using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Dmi.Scripts.Player
{
    [System.Serializable]
    public class FirstPersonCameraController : IPlayerComponent//, ILateTickable
    {
        // [SerializeField] CinemachineVirtualCamera _virtualCamera;
        // [SerializeField] CinemachineInputProvider _inputProvider;
         [SerializeField] Transform _cameraTarget;
        // [SerializeField] AnimationCurve _defaultTransitionCurve;
        // [SerializeField] float _minCameraSpeed = 30f;
        // [SerializeField] float _maxCameraSpeed = 230f;
        //
        // Transform _cameraParent;
        // Transform _cameraRotationTemplate;
        // CinemachinePOV _cinemachinePOV;
        // Quaternion _cameraRotation;
        // CameraCharacteristics _currentCharacteristics;
        //
        // Tween _positionTween;
        // Tween _rotationTween;
        //
        // public Quaternion GetCameraRotation => _cameraRotation;
         public Transform GetCameraTransform => _cameraTarget;
        //
        // [Zenject.Inject]
        // private void Construct(MainCameraController mainCameraController, PlayerCore playerCore)
        // {
        //     _cameraParent = _cameraTarget.parent;
        //     _cameraRotationTemplate = mainCameraController.Camera.transform;
        //     _cinemachinePOV = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        //
        //     UpdateDefaultCharacteristics();
        // }
        //
        // public void OnSensitivityChanged(float newValue01)
        // {
        //     float newSpeed = Mathf.Lerp(_minCameraSpeed, _maxCameraSpeed, newValue01);
        //
        //     if (_cinemachinePOV != null)
        //     {
        //         _cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = newSpeed;
        //         _cinemachinePOV.m_VerticalAxis.m_MaxSpeed = newSpeed;
        //     }
        // }
        //
        // private void UpdateDefaultCharacteristics()
        // {
        //     var defaultCameraPosition = _cameraTarget.localPosition;
        //
        //     var defaultCharacteristics = new CameraCharacteristics(
        //         cameraLocalPosition: defaultCameraPosition,
        //         cameraGlobalPosition: null,
        //         enableCameraRotation: true,
        //         changeRotationBorders: true,
        //         horizontalMinValue: _cinemachinePOV.m_HorizontalAxis.m_MinValue,
        //         horizontalMaxValue: _cinemachinePOV.m_HorizontalAxis.m_MaxValue,
        //         verticalMinValue: _cinemachinePOV.m_VerticalAxis.m_MinValue,
        //         verticalMaxValue: _cinemachinePOV.m_VerticalAxis.m_MaxValue);
        //
        //     CameraCharacteristics.SetDefaultCharacteristics(defaultCharacteristics);
        // }
        //
        // public void DisableRotation()
        // {
        //     if (_cinemachinePOV) _cinemachinePOV.enabled = false;
        //     else Debug.LogError($"{GetType()} _cinemachinePOV is not enabled");
        // }
        //
        // public void EnableRotation()
        // {
        //     if (_cinemachinePOV) _cinemachinePOV.enabled = true;
        //     else Debug.LogError($"{GetType()} _cinemachinePOV is not enabled");
        // }
        //
        // public void LateTick()
        // {
        //     _cameraRotation = _cameraRotationTemplate.rotation;
        //     _cameraTarget.rotation = Quaternion.Euler(0f, _cameraRotationTemplate.rotation.eulerAngles.y, 0f);
        // }
        //
        // public void SetDefaultCharacteristics(float transitionTime, AnimationCurve transitionCurve = null)
        // {
        //     SetAnyCharacteristics(CameraCharacteristics.GetDefaultCharacteristics(), transitionTime, transitionCurve);
        // }
        //
        // public void SetAnyCharacteristics(CameraCharacteristics characteristics,
        //     float transitionTime,
        //     AnimationCurve transitionCurve = null,
        //     Vector3? lookAtGlobalPosition = null)
        // {
        //     if (transitionTime <= 0f)
        //     {
        //         ApplyCharacteristicsInstantly(characteristics, lookAtGlobalPosition);
        //         return;
        //     }
        //
        //     UpdateCharacteristics(characteristics, transitionTime, transitionCurve, lookAtGlobalPosition);
        // }
        //
        // private void UpdateCharacteristics(
        //     CameraCharacteristics newCharacteristics,
        //     float transitionTime,
        //     AnimationCurve transitionCurve = null,
        //     Vector3? lookAtGlobalPosition = null)
        // {
        //     _currentCharacteristics = newCharacteristics;
        //
        //     var finalPosition = UpdateCameraTransform(newCharacteristics, transitionTime, transitionCurve);
        //     UpdateCameraRotation(newCharacteristics, finalPosition, transitionTime, transitionCurve,
        //         lookAtGlobalPosition);
        // }
        //
        // private Vector3 UpdateCameraTransform(CameraCharacteristics newCharacteristics,
        //     float transitionTime,
        //     AnimationCurve transitionCurve = null)
        // {
        //     _positionTween?.Kill();
        //
        //     Vector3 targetGlobalPosition = _cameraTarget.position;
        //
        //     if (newCharacteristics.CameraLocalPosition.HasValue)
        //     {
        //         _cameraTarget.SetParent(_cameraParent);
        //
        //         _positionTween = _cameraTarget.DOLocalMove(newCharacteristics.CameraLocalPosition.Value, transitionTime)
        //             .SetEase(transitionCurve ?? _defaultTransitionCurve);
        //
        //         targetGlobalPosition = _cameraParent.TransformPoint(newCharacteristics.CameraLocalPosition.Value);
        //     }
        //     else if (newCharacteristics.CameraGlobalPosition.HasValue)
        //     {
        //         _cameraTarget.SetParent(null);
        //
        //         _positionTween = _cameraTarget.DOMove(newCharacteristics.CameraGlobalPosition.Value, transitionTime)
        //             .SetEase(transitionCurve ?? _defaultTransitionCurve)
        //             .OnKill(() => { _cameraTarget.SetParent(_cameraParent); });
        //
        //         targetGlobalPosition = newCharacteristics.CameraGlobalPosition.Value;
        //     }
        //
        //     return targetGlobalPosition;
        // }
        //
        // private void UpdateCameraRotation(CameraCharacteristics newCharacteristics,
        //     Vector3 endPosition,
        //     float transitionTime,
        //     AnimationCurve transitionCurve = null,
        //     Vector3? lookAtGlobalPosition = null)
        // {
        //     _cinemachinePOV.m_HorizontalAxis.m_MinValue = newCharacteristics.HorizontalMinValue;
        //     _cinemachinePOV.m_HorizontalAxis.m_MaxValue = newCharacteristics.HorizontalMaxValue;
        //     _cinemachinePOV.m_VerticalAxis.m_MinValue = newCharacteristics.VerticalMinValue;
        //     _cinemachinePOV.m_VerticalAxis.m_MaxValue = newCharacteristics.VerticalMaxValue;
        //
        //     if (lookAtGlobalPosition.HasValue)
        //     {
        //         _rotationTween?.Kill();
        //
        //         DisableCameraRotation();
        //
        //         if (newCharacteristics.CameraGlobalPosition.HasValue)
        //         {
        //             endPosition = newCharacteristics.CameraGlobalPosition.Value;
        //         }
        //
        //         var direction = (lookAtGlobalPosition.Value - endPosition).normalized;
        //         var targetRotation = Quaternion.LookRotation(direction);
        //
        //         var currentYaw = _cinemachinePOV.m_HorizontalAxis.Value;
        //         var currentPitch = _cinemachinePOV.m_VerticalAxis.Value;
        //
        //         var targetYaw = targetRotation.eulerAngles.y;
        //         var targetPitch = targetRotation.eulerAngles.x;
        //
        //         targetYaw = Mathf.LerpAngle(currentYaw, targetYaw, 1);
        //         targetPitch = Mathf.LerpAngle(currentPitch, targetPitch, 1);
        //
        //         _rotationTween = DOTween.Sequence()
        //             .Append(DOTween.To(
        //                 () => _cinemachinePOV.m_HorizontalAxis.Value,
        //                 x => _cinemachinePOV.m_HorizontalAxis.Value = x,
        //                 targetYaw,
        //                 transitionTime).SetEase(transitionCurve ?? _defaultTransitionCurve))
        //             .Join(DOTween.To(
        //                 () => _cinemachinePOV.m_VerticalAxis.Value,
        //                 x => _cinemachinePOV.m_VerticalAxis.Value = x,
        //                 targetPitch,
        //                 transitionTime).SetEase(transitionCurve ?? _defaultTransitionCurve))
        //             .OnKill(() =>
        //             {
        //                 if (_currentCharacteristics.EnableCameraRotation) EnableCameraRotation();
        //                 else DisableCameraRotation();
        //             });
        //     }
        //     else
        //     {
        //         if (_currentCharacteristics.EnableCameraRotation) EnableCameraRotation();
        //         else DisableCameraRotation();
        //     }
        // }
        //
        // private void ApplyCharacteristicsInstantly(CameraCharacteristics newCharacteristics,
        //     Vector3? lookAtGlobalPosition)
        // {
        //     _cinemachinePOV.m_HorizontalAxis.m_MinValue = newCharacteristics.HorizontalMinValue;
        //     _cinemachinePOV.m_HorizontalAxis.m_MaxValue = newCharacteristics.HorizontalMaxValue;
        //     _cinemachinePOV.m_VerticalAxis.m_MinValue = newCharacteristics.VerticalMinValue;
        //     _cinemachinePOV.m_VerticalAxis.m_MaxValue = newCharacteristics.VerticalMaxValue;
        //
        //     if (newCharacteristics.CameraLocalPosition.HasValue)
        //     {
        //         _cameraTarget.localPosition = newCharacteristics.CameraLocalPosition.Value;
        //     }
        //     else if (newCharacteristics.CameraGlobalPosition.HasValue)
        //     {
        //         _cameraTarget.position = newCharacteristics.CameraGlobalPosition.Value;
        //     }
        //
        //     if (lookAtGlobalPosition.HasValue)
        //     {
        //         _cameraRotationTemplate.rotation =
        //             Quaternion.LookRotation(lookAtGlobalPosition.Value - _cameraTarget.position);
        //     }
        //
        //     if (newCharacteristics.EnableCameraRotation) EnableCameraRotation();
        //     else DisableCameraRotation();
        // }
        //
        // private void EnableCameraRotation()
        // {
        //     _inputProvider.enabled = true;
        // }
        //
        // private void DisableCameraRotation()
        // {
        //     _inputProvider.enabled = false;
        // }
        //
        // [System.Serializable]
        // public struct CameraCharacteristics
        // {
        //     static CameraCharacteristics _defaultCharacteristics;
        //
        //     public Vector3? CameraLocalPosition;
        //     public Vector3? CameraGlobalPosition;
        //
        //     public bool EnableCameraRotation;
        //
        //     public float HorizontalMinValue;
        //     public float HorizontalMaxValue;
        //
        //     public float VerticalMinValue;
        //     public float VerticalMaxValue;
        //
        //     public CameraCharacteristics(
        //         Vector3? cameraLocalPosition,
        //         Vector3? cameraGlobalPosition,
        //         bool enableCameraRotation,
        //         bool changeRotationBorders,
        //         float horizontalMinValue,
        //         float horizontalMaxValue,
        //         float verticalMinValue,
        //         float verticalMaxValue)
        //     {
        //         CameraLocalPosition = cameraLocalPosition;
        //         CameraGlobalPosition = cameraGlobalPosition;
        //         EnableCameraRotation = enableCameraRotation;
        //         HorizontalMinValue = horizontalMinValue;
        //         HorizontalMaxValue = horizontalMaxValue;
        //         VerticalMinValue = verticalMinValue;
        //         VerticalMaxValue = verticalMaxValue;
        //     }
        //
        //     public static CameraCharacteristics GetDefaultCharacteristics()
        //         => _defaultCharacteristics;
        //
        //     public static void SetDefaultCharacteristics(CameraCharacteristics defaultCharacteristics)
        //         => _defaultCharacteristics = defaultCharacteristics;
        // }
    }
}