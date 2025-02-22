using System;
using System.Collections.Generic;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Object = UnityEngine.Object;

namespace GameObjects.UI.Workshop.ConstructPartsShop.TextureCreators
{
    [Serializable]
    public class CameraTextureCreator
    {
        [HideLabel] [SerializeField] Camera _camera;
        
        readonly Queue<RenderTextureRequest> _dataQueue = new();
        bool _isProcessingQueue;

        private void Awake()
        {
            _camera.enabled = false;
            _camera.gameObject.SetActive(true);
        }

        public void AddDataToQueue(RenderTextureRequest request)
        {
            request.ObjectToRender.SetActive(false);

            _dataQueue.Enqueue(request);

            if (!_isProcessingQueue)
            {
                _isProcessingQueue = true;

                if (_camera == null) return;

                CreateTextureLoop();
            }
        }

        private void CreateTextureLoop()
        {
            while (_dataQueue.Count > 0)
            {
                var data = _dataQueue.Dequeue();

                ObjectToRenderSetting(data);

                var texture = new RenderTexture(data.TextureSize.x, data.TextureSize.y, 16,GraphicsFormat.R8G8B8A8_UNorm);
                
                _camera.targetTexture = texture;
                
                _camera.Render();

                if (data.Callback != null)
                {
                    data.Callback.Invoke(texture, data.ObjectToRender);
                }
                else
                {
                    Object.Destroy(texture);
                    Debug.LogError($"No callback action in {GetType().Name}");
                }

                //data.ObjectToRender.SetActive(false);
            }

            _isProcessingQueue = false;

            _camera.targetTexture = null;
        }

        private void ObjectToRenderSetting(RenderTextureRequest request)
        {
            Debug.Log(true);
            request.ObjectToRender.SetActive(true);

            request.ObjectToRender.transform.SetParent(_camera.transform);
            request.ObjectToRender.transform.SetLocalPositionAndRotation(request.PositionOffset, request.RotationOffset);
        }

        public readonly struct RenderTextureRequest
        {
            public readonly GameObject ObjectToRender;
            public readonly Vector3 PositionOffset;
            public readonly Quaternion RotationOffset;
            public readonly Vector2Int TextureSize;
            public readonly Action<RenderTexture, GameObject> Callback;

            public RenderTextureRequest(
                GameObject objectToRender,
                Vector3 positionOffset,
                Quaternion rotationOffset,
                Vector2Int textureSize,
                Action<RenderTexture, GameObject> callback)
            {
                ObjectToRender = objectToRender;
                PositionOffset = positionOffset;
                RotationOffset = rotationOffset;
                TextureSize = textureSize;
                Callback = callback;
            }
        }
    }
}