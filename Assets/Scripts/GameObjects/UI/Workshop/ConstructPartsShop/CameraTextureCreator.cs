using System;
using System.Collections.Generic;
using System.Threading;
using Additional;
using Cysharp.Threading.Tasks;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;

namespace GameObjects.UI.Workshop.ConstructPartsShop
{
    public class CameraTextureCreator : MonoBehaviour
    {
        [HideLabel] [SerializeField] Camera _camera;

        readonly Queue<RenderTextureData> _dataQueue = new();

        CancellationTokenSource _cts;

        private void Awake()
        {
            _camera.gameObject.SetActive(false);
        }

        public void AddDataToQueue(RenderTextureData data)
        {
            _dataQueue.Enqueue(data);

            if (_cts == null)
            {
                _cts = new();

                _camera.gameObject.SetActive(true);
                CreateTextureLoop(_cts.Token).Forget();
            }
        }

        private async UniTaskVoid CreateTextureLoop(CancellationToken token)
        {
            while (_dataQueue.Count > 0)
            {
                var data = _dataQueue.Dequeue();

                ObjectToRenderSetting(data);

                var texture = new RenderTexture(data.TextureSize.x, data.TextureSize.y, 0);
                _camera.targetTexture = texture;

                await UniTask.Yield(cancellationToken: token);

                if (data.Callback != null)
                {
                    data.Callback.Invoke(texture, data.ObjectToRender);
                }
                else
                {
                    Destroy(texture);
                    Debug.LogError($"No callback action {this.GetType().Name}");
                }
            }
            
            ClearToken();
            _camera.gameObject.SetActive(false);
        }

        private void ObjectToRenderSetting(RenderTextureData data)
        {
            data.ObjectToRender.transform.SetParent(_camera.transform);

            data.ObjectToRender.transform.SetLocalPositionAndRotation(data.PositionOffset, data.RotationOffset);
        }

        private void ClearToken() => ClearTokenSupport.ClearToken(ref _cts);

        private void OnDestroy()
        {
            ClearToken();
        }

        public readonly struct RenderTextureData
        {
            public readonly GameObject ObjectToRender;
            public readonly Vector3 PositionOffset;
            public readonly Quaternion RotationOffset;
            public readonly Vector2Int TextureSize;
            public readonly Action<RenderTexture, GameObject> Callback;

            public RenderTextureData(GameObject objectToRender,
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