using R3;
using UnityEngine;

namespace Dmi.Scripts.Player
{
    public class GroundChecker : MonoBehaviour, IDistributedUpdatable
    {
        [SerializeField] Transform _rayStartPosition;
        [SerializeField] float _rayLength = 0.2f;
        [SerializeField] LayerMask _groundMask = ~0;
        EnumUpdateFrequency _updateFrequency = EnumUpdateFrequency.Every4Frames;

        static readonly RaycastHit[] _raycastHits = new RaycastHit[1];
        DistributedUpdateLoop _updateLoop;

        public ReadOnlyReactiveProperty<bool> IsGrounded => _isGrounded;
        private readonly ReactiveProperty<bool> _isGrounded = new(false);


        [Zenject.Inject]
        private void Construct(DistributedUpdateLoop updateLoop)
        {
            _updateLoop = updateLoop;
        }

        private void OnEnable()
        {
            _updateLoop.Register(_updateFrequency, this);
        }

        private void OnDisable()
        {
            _updateLoop.Unregister(_updateFrequency, this);
        }

        public void DistributedUpdate()
        {
            Vector3 origin = _rayStartPosition != null ? _rayStartPosition.position : transform.position;
            Vector3 direction = Vector3.down;

            int hitCount = Physics.RaycastNonAlloc(origin, direction, _raycastHits, _rayLength, _groundMask,
                QueryTriggerInteraction.Ignore);

            _isGrounded.Value = hitCount > 0;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 origin = _rayStartPosition != null ? _rayStartPosition.position : transform.position;
            Vector3 direction = Vector3.down * _rayLength;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, origin + direction);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(origin + direction, 0.02f);
        }
#endif
        private void OnDestroy()
        {
            _updateLoop.Unregister(_updateFrequency, this);
        }
    }
}