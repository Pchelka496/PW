using UnityEngine;

namespace GameObjects.CameraControllers.Workshop
{
    public class WorkshopCameraController : MonoBehaviour
    {
        [SerializeField] OrbitalFollowController _orbitalFollowController; 
        [SerializeField] CameraMovementController _cameraMovementController;

        public CameraMovementController MovementController => _cameraMovementController;

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