using System;
using System.Threading;
using Additional;
using UnityEngine;

namespace GameObjects.CameraControllers.Workshop
{
    public class WorkshopCameraController : MonoBehaviour
    {
        [SerializeField] OrbitalFollowController _orbitalFollowController;
        [SerializeField] CameraTrackingTargetController _cameraTrackingTargetController;

        private void Reset()
        {
            if (!gameObject.TryGetComponent<OrbitalFollowController>(out _orbitalFollowController))
            {
                _orbitalFollowController = gameObject.AddComponent<OrbitalFollowController>();
            }

            if (!gameObject.TryGetComponent<CameraTrackingTargetController>(out _cameraTrackingTargetController))
            {
                _cameraTrackingTargetController = gameObject.AddComponent<CameraTrackingTargetController>();
            }
        }
    }
}