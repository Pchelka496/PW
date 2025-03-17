using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using Additional;
using Cysharp.Threading.Tasks;
using UnityEngine.Experimental.Rendering;

namespace GameObjects.UI.Workshop.ConstructPartsShop.TextureCreators
{
    [Serializable]
    public class CameraTextureAtlasCreator
    {
        readonly Queue<TextureRequest> _requestsQueue = new();
        CameraTextureCreator _textureCreator;
        CancellationTokenSource _cts;

        public void Initialize(CameraTextureCreator textureCreator)
        {
            _textureCreator = textureCreator;
        }

        public void RequestTextureAtlas(
            GameObject[] objectsToRender,
            Vector2Int sizes,
            Vector3[] positions,
            Quaternion[] rotations,
            Action<RenderTexture, Rect[], GameObject[]> callback)
        {
            if (objectsToRender.Length != positions.Length ||
                objectsToRender.Length != rotations.Length)
            {
                Debug.LogError("CameraTextureAtlasCreator: Arrays length mismatch!");
                return;
            }

            var request = new TextureRequest(objectsToRender, sizes, positions, rotations, callback);
            _requestsQueue.Enqueue(request);

            if (_cts == null)
            {
                _cts = new();
                ProcessQueue(_cts.Token).Forget();
            }
        }

        private async UniTaskVoid ProcessQueue(CancellationToken token)
        {
            while (_requestsQueue.Count > 0)
            {
                var request = _requestsQueue.Dequeue();
                var objects = request.Objects;
                var textureSize = request.TextureSize;
                var remainingTextures = objects.Length;

                var atlasSize = CalculateAtlasSize(textureSize, objects.Length);
                
                var renderTexture = new RenderTexture(atlasSize.x, atlasSize.y, 0, RenderTextureFormat.ARGB32);
                renderTexture.depthStencilFormat = GraphicsFormat.None;
                
                RenderTexture.active = renderTexture;
                GL.Clear(true, true, Color.clear);

                var uvRects = new Rect[objects.Length];
                var xOffset = 0;

                for (int i = 0; i < objects.Length; i++)
                {
                    var objectSize = textureSize;
                    var localX = xOffset;
                    var localY = 0;
                    xOffset += objectSize.x;

                    uvRects[i] = new Rect(
                        (float)localX / atlasSize.x, 0,
                        (float)objectSize.x / atlasSize.x, (float)objectSize.y / atlasSize.y);

                    _textureCreator.AddDataToQueue(new CameraTextureCreator.RenderTextureRequest(
                        objects[i], request.Positions[i], request.Rotations[i], objectSize, (tex, obj) =>
                        {
                            Graphics.CopyTexture(tex, 0, 0, 0, 0, objectSize.x, objectSize.y, renderTexture, 0, 0,
                                localX, localY);
                            tex.Release();
                            remainingTextures--;
                        }));
                }

                await UniTask.WaitUntil(() => remainingTextures == 0, cancellationToken: token);

                request.Callback?.Invoke(renderTexture, uvRects, objects);
                RenderTexture.active = null;
                renderTexture = null;
            }

            ClearToken();
        }

        private Vector2Int CalculateAtlasSize(Vector2Int size, int count)
        {
            int width = 0, height = 0;
            
            for (var i = 0; i < count; i++)
            {
                width += size.x;
                height = Mathf.Max(height, size.y);
            }

            return new Vector2Int(width, height);
        }

        private void ClearToken() => ClearTokenSupport.ClearToken(ref _cts);

        private readonly struct TextureRequest
        {
            public readonly GameObject[] Objects;
            public readonly Vector2Int TextureSize;
            public readonly Vector3[] Positions;
            public readonly Quaternion[] Rotations;
            public readonly Action<RenderTexture, Rect[], GameObject[]> Callback;

            public TextureRequest(
                GameObject[] objects,
                Vector2Int textureSize,
                Vector3[] positions,
                Quaternion[] rotations,
                Action<RenderTexture, Rect[], GameObject[]> callback)
            {
                Objects = objects;
                TextureSize = textureSize;
                Positions = positions;
                Rotations = rotations;
                Callback = callback;
            }
        }
    }
}