using System;
using System.Threading;
using Additional;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameObjects.CameraControllers.Workshop
{
    public class WorkshopCameraController : MonoBehaviour
    {
        [SerializeField] OrbitalFollowController _orbitalFollowController;
        [FormerlySerializedAs("_cameraTrackingTargetController")] [SerializeField] CameraMovementController _cameraMovementController;

        private void Reset()
        {
            if (!gameObject.TryGetComponent<OrbitalFollowController>(out _orbitalFollowController))
            {
                _orbitalFollowController = gameObject.AddComponent<OrbitalFollowController>();
            }

            if (!gameObject.TryGetComponent<CameraMovementController>(out _cameraMovementController))
            {
                _cameraMovementController = gameObject.AddComponent<CameraMovementController>();
            }
        }
    }
}