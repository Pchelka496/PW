using System.Collections.Generic;
using UnityEngine;

namespace GameObjects.Construct.Workshop
{
    public class CameraBordersColliders
    {
        Transform _borderParentTransform;
        readonly List<GameObject> _colliders = new();
        Vector3 _centerPosition;
        Vector3 _edgeDistance;

        public CameraBordersColliders Initialize(
            Transform borderParentTransform,
            Vector3 centerPosition,
            Vector3 edgeDistance)
        {
            _borderParentTransform = borderParentTransform;
            _edgeDistance = edgeDistance;
            _centerPosition = centerPosition;

            return this;
        }

        public void CreateColliders()
        {
            CreateBorderCollider("TopBorder",
                position: _centerPosition + Vector3.up * _edgeDistance.y,
                size: new Vector3(_edgeDistance.x * 2, 1, _edgeDistance.z * 2));

            CreateBorderCollider("BottomBorder",
                position: _centerPosition + Vector3.down * _edgeDistance.y,
                size: new Vector3(_edgeDistance.x * 2, 1, _edgeDistance.z * 2));

            CreateBorderCollider("LeftBorder",
                position: _centerPosition + Vector3.left * _edgeDistance.x,
                size: new Vector3(1, _edgeDistance.y * 2, _edgeDistance.z * 2));

            CreateBorderCollider("RightBorder",
                position: _centerPosition + Vector3.right * _edgeDistance.x,
                size: new Vector3(1, _edgeDistance.y * 2, _edgeDistance.z * 2));

            CreateBorderCollider("FrontBorder",
                position: _centerPosition + Vector3.forward * _edgeDistance.z,
                size: new Vector3(_edgeDistance.x * 2, _edgeDistance.y * 2, 1));

            CreateBorderCollider("BackBorder",
                position: _centerPosition + Vector3.back * _edgeDistance.z,
                size: new Vector3(_edgeDistance.x * 2, _edgeDistance.y * 2, 1));
        }

        private void CreateBorderCollider(string name, Vector3 position, Vector3 size)
        {
            var borderObj = new GameObject(name);
            borderObj.transform.parent = _borderParentTransform;
            borderObj.layer = LayerMask.NameToLayer("Default");

            var collider = borderObj.AddComponent<BoxCollider>();
            borderObj.transform.position = position;
            collider.size = size;

            _colliders.Add(borderObj);
        }
    }
}