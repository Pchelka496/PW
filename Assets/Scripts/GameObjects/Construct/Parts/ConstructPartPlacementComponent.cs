using System;
using System.Collections.Generic;
using GameObjects.Construct.Workshop;
using R3;
using UnityEngine;

namespace GameObjects.Construct.Parts
{
    public class ConstructPartPlacementComponent : MonoBehaviour
    {
        [SerializeField] Collider[] _colliders;
        [SerializeField] PlacementMaterialSwitcher _materialSwitcher;

        IDisposable _placementOpportunitySubscription;
        HashSet<Collider> _collidingObjects = new();

        public ReactiveProperty<int> CollisionCount { get; private set; } = new();

        private void Awake()
        {
            _materialSwitcher.Initialize();
        }

        public void StartPlacement(ConstructPartPlacementController placementController)
        {
            _placementOpportunitySubscription = placementController.PlacementOpportunity.Subscribe((opportunity) =>
            {
                if (opportunity)
                {
                    _materialSwitcher.SetDefaultMaterials();
                }
                else
                {
                    _materialSwitcher.SetInvalidMaterial();
                }
            });

            foreach (var collider in _colliders)
            {
                collider.isTrigger = true;
            }
        }

        public void PlacementFinally()
        {
            foreach (var collider in _colliders)
            {
                collider.isTrigger = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_collidingObjects.Add(other))
            {
                CollisionCount.Value++;
                _materialSwitcher.SetDefaultMaterials();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_collidingObjects.Remove(other))
            {
                CollisionCount.Value--;

                if (CollisionCount.Value == 0)
                {
                    _materialSwitcher.SetInvalidMaterial();
                }
            }
        }

        private void OnDestroy()
        {
            _placementOpportunitySubscription?.Dispose();
        }

        [Serializable]
        private struct PlacementMaterialSwitcher
        {
            [SerializeField] MeshRenderer[] _meshRenderers;
            [SerializeField] Material _invalidMaterial;
            Material[][] _defaultMaterials;

            public void Initialize()
            {
                _defaultMaterials = new Material[_meshRenderers.Length][];

                for (int i = 0; i < _meshRenderers.Length; i++)
                {
                    var originalMats = _meshRenderers[i].materials;
                    _defaultMaterials[i] = new Material[originalMats.Length];
                    Array.Copy(originalMats, _defaultMaterials[i], originalMats.Length);
                }
            }

            public void SetDefaultMaterials()
            {
                for (int i = 0; i < _meshRenderers.Length; i++)
                {
                    _meshRenderers[i].materials = _defaultMaterials[i];
                }
            }

            public void SetInvalidMaterial()
            {
                foreach (var meshRenderer in _meshRenderers)
                {
                    var materials = meshRenderer.materials;
                    for (var i = 0; i < materials.Length; i++)
                    {
                        materials[i] = _invalidMaterial;
                    }

                    meshRenderer.materials = materials;
                }
            }
        }
    }
}