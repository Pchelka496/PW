using UnityEngine;
using Zenject;

namespace Dmi.Scripts.Player
{
    public class PlayerCore : MonoBehaviour
    {
        [SerializeField] PlayerFocusController _playerFocusController;
        [SerializeField] PlayerMovementController _playerMovementController;
        [SerializeField] FirstPersonCameraController _firstFirstPersonCameraController;

        [Space] [SerializeField] LayerMask _collisionOnExcludeMask;
        [SerializeField] LayerMask _collisionOffExcludeMask;
        [SerializeField] CharacterController _characterController;

        IPlayerComponent[] _components;
        DiContainer _container;

        public PlayerFocusController FocusController => _playerFocusController;
        public FirstPersonCameraController FirstPersonCameraController => _firstFirstPersonCameraController;
        public PlayerMovementController MovementController => _playerMovementController;

        [Zenject.Inject]
        private void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Awake()
        {
            _components = new IPlayerComponent[]
                { FocusController, MovementController, FirstPersonCameraController };

            var tickableManager = _container.Resolve<TickableManager>();

            foreach (var component in _components)
            {
                _container.Inject(component);

                if (component is ITickable tickable)
                    tickableManager.Add(tickable);
                if (component is ILateTickable lateTickable)
                    tickableManager.AddLate(lateTickable);
                if (component is IFixedTickable fixedTickable)
                    tickableManager.AddFixed(fixedTickable);
            }

            transform.rotation = Quaternion.identity;
            EnablePhysics();
        }

        public void DisablePhysics()
        {
            _characterController.excludeLayers = _collisionOffExcludeMask;
            _playerMovementController.UseGravity = false;
        }

        public void EnablePhysics()
        {
            _characterController.excludeLayers = _collisionOnExcludeMask;
            _playerMovementController.UseGravity = true;
        }

        private void OnDestroy()
        {
            _playerFocusController?.Dispose();
            _playerMovementController?.Dispose();
        }
    }

    public interface IPlayerComponent
    {
    }
}