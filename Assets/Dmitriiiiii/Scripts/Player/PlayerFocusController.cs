using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Dmi.Scripts.Player
{
    [Serializable]
    public class PlayerFocusController : IPlayerComponent, IDistributedUpdatable, IDisposable
    {
        const EnumUpdateFrequency UPDATE_FREQUENCY = EnumUpdateFrequency.Every4Frames;
        const int HIT_BUFFER_SIZE = 16;

        [SerializeField] Characteristics _characteristics;
        [SerializeField] PlayerInteractProcessor _interactProcessor;

        DistributedUpdateLoop _distributedUpdateLoop;
        Camera _camera;

        IInteractIndicator _interactIndicator;

        IFocused _focused;
        IInteractable _interactable;

        readonly RaycastHit[] _hitsBuffer = new RaycastHit[HIT_BUFFER_SIZE];

        public IFocused Focused => _focused;

        public IInteractable Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                _interactProcessor.SetIInteractible(_interactable);
            }
        }

        [Inject]
        private void Construct(
            MainCameraController mainCameraController,
            PlayerSettings playerSettings,
            DistributedUpdateLoop updateLoop,
            IInteractIndicator interactIndicator)
        {
            _characteristics = playerSettings.InteractionCharacteristics;
            _camera = mainCameraController.Camera;
            
            _interactProcessor.Initialize(interactIndicator);

            _distributedUpdateLoop = updateLoop;
            _distributedUpdateLoop.Register(UPDATE_FREQUENCY, this);
        }

        public void DistributedUpdate()
        {
            CheckForFocus();
        }

        private void CheckForFocus()
        {
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            int hitCount = Physics.RaycastNonAlloc(
                ray,
                _hitsBuffer,
                _characteristics.InteractionDistance,
                _characteristics.InteractableLayer);

            if (hitCount > 0)
            {
                Array.Sort(_hitsBuffer, 0, hitCount, HitComparer.CreateNew);

                var hit = _hitsBuffer[0];
                var rb = hit.collider.attachedRigidbody;

                if (rb != null)
                {
                    IFocused newFocused = null;
                    foreach (var component in rb.GetComponents<MonoBehaviour>())
                    {
                        if (component is IFocused focused)
                        {
                            newFocused = focused;
                            break;
                        }
                    }

                    if (newFocused != null)
                    {
                        if (_focused != newFocused)
                        {
                            _focused?.OnFocusExit();
                            _focused = newFocused;
                            _focused.OnFocusEnter();
                        }

                        UpdateInterfacesFromComponents(rb);
                        return;
                    }
                }
            }

            if (_focused != null)
            {
                _focused.OnFocusExit();
                _focused = null;
                Interactable = null;
            }
        }

        private void UpdateInterfacesFromComponents(Rigidbody rb)
        {
            IInteractable newInteractable = null;

            foreach (var component in rb.GetComponents<MonoBehaviour>())
            {
                if (newInteractable == null && component is IInteractable interactable)
                    newInteractable = interactable;
                
                if (newInteractable != null)
                    break;
            }

            if (_interactable != newInteractable)
                Interactable = newInteractable;
        }

        [Serializable]
        public struct Characteristics
        {
            public LayerMask InteractableLayer;
            [Range(0f, 100f)] public float InteractionDistance;
        }

        public void Dispose()
        {
            _distributedUpdateLoop.Unregister(UPDATE_FREQUENCY, this);
        }


        public readonly struct HitComparer : IComparer<RaycastHit>
        {
            public static readonly HitComparer CreateNew = new();
            public int Compare(RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance);
        }
    }
}