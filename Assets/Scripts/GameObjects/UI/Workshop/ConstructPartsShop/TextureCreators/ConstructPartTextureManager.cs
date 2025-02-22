using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameObjects.Construct;
using GameObjects.Construct.Parts;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using ScriptableObjects;
using UnityEngine;

namespace GameObjects.UI.Workshop.ConstructPartsShop.TextureCreators
{
    public class ConstructPartTextureManager : MonoBehaviour
    {
        [SerializeField] Vector2Int _textureSize;
        [SerializeField] CameraTextureCreator _textureCreator;
        [HideLabel] [SerializeField] CameraTextureAtlasCreator _atlasCreator;

        ConstructPartsDataLoader _constructPartsDataLoader;
        ConstructPartFactory _constructPartFactory;

        ConstructPartData[] _constructPartData;
        ConstructPartCore[] _renderedPart;
        RenderTexture[] _renderTextures;
        readonly Dictionary<string, TextureAtlasData> _textureAtlases = new();

        [Zenject.Inject]
        private void Construct(
            ConstructPartsDataLoader constructPartsDataLoader,
            ConstructPartFactory constructPartFactory)
        {
            _constructPartsDataLoader = constructPartsDataLoader;
            _constructPartFactory = constructPartFactory;
            _atlasCreator.Initialize(_textureCreator);
        }

        private void Awake()
        {
            LoadConstructPartsData().Forget();
        }

        private async UniTaskVoid LoadConstructPartsData()
        {
            _constructPartData = await _constructPartsDataLoader.GetAllPartsAsync();
            _renderedPart = new ConstructPartCore[_constructPartData.Length];
            _renderTextures = new RenderTexture[_constructPartData.Length];
        }

        public async UniTaskVoid GetTexture(
            ConstructPartData partData,
            Action<RenderTexture, ConstructPartData> callback)
        {
            await UniTask.WaitUntil(() => _renderTextures != null);

            if (_renderTextures[partData.LoadID] != null)
            {
                callback?.Invoke(_renderTextures[partData.LoadID], partData);
                return;
            }

            var renderObject = await GetRenderedPart(partData);
            _textureCreator.AddDataToQueue(new CameraTextureCreator.RenderTextureRequest(
                renderObject.gameObject,
                partData.LocalPositionForRenderTexture,
                partData.RotationForRenderTexture,
                _textureSize,
                (texture, renderObject) =>
                {
                    _renderTextures[partData.LoadID] = texture;
                    callback?.Invoke(texture, partData);
                }));
        }

        public async UniTaskVoid GetTextureAtlas(
            ConstructPartData[] partData,
            Action<RenderTexture, Rect[], ConstructPartData[]> callback)
        {
            var key = GetPartsKey(partData);

            if (_textureAtlases.TryGetValue(key, out var cachedAtlas) && !cachedAtlas.IsDefault)
            {
                callback?.Invoke(cachedAtlas.RenderTexture, cachedAtlas.Rects, partData);
                return;
            }

            var parts = await GetRenderedPartsAsync(partData);
            var renderObjects = new GameObject[partData.Length];
            var positions = new Vector3[partData.Length];
            var rotations = new Quaternion[partData.Length];

            for (int i = 0; i < partData.Length; i++)
            {
                renderObjects[i] = parts[i].gameObject;
                positions[i] = partData[i].LocalPositionForRenderTexture;
                rotations[i] = partData[i].RotationForRenderTexture;
            }

            _atlasCreator.RequestTextureAtlas(renderObjects, _textureSize, positions, rotations,
                (texture, rects, objs) =>
                {
                    var atlasData = new TextureAtlasData(
                        texture,
                        rects,
                        objs.Select(o => (uint)o.GetInstanceID()).ToArray());

                    _textureAtlases[key] = atlasData;
                    callback?.Invoke(atlasData.RenderTexture, atlasData.Rects, partData);
                });
        }

        private async UniTask<ConstructPartCore> GetRenderedPart(ConstructPartData partData)
        {
            await UniTask.WaitUntil(() => _renderedPart != null);
            
            if (_renderedPart[partData.LoadID] != null)
            {
                return _renderedPart[partData.LoadID];
            }

            var renderedPart = Instantiate(await _constructPartFactory.GetConstructPart(partData));
            _renderedPart[partData.LoadID] = renderedPart;

            return renderedPart;
        }

        private async UniTask<ConstructPartCore[]> GetRenderedPartsAsync(ConstructPartData[] partData)
        {
            var tasks = partData.Select(GetRenderedPart).ToArray();
            return await UniTask.WhenAll(tasks);
        }

        private string GetPartsKey(ConstructPartData[] parts)
        {
            const int UINT_MAX_LENGTH = 10; //4294967295

            if (parts == null || parts.Length == 0)
                return string.Empty;

            Array.Sort(parts, (a, b) => a.LoadID.CompareTo(b.LoadID));

            var estimatedSize = parts.Length * UINT_MAX_LENGTH + (parts.Length - 1);

            var builder = new System.Text.StringBuilder(estimatedSize);

            for (int i = 0; i < parts.Length; i++)
            {
                builder.Append(parts[i].LoadID);
                if (i < parts.Length - 1)
                    builder.Append('_');
            }

            return builder.ToString();
        }

        private readonly struct TextureAtlasData
        {
            public readonly RenderTexture RenderTexture;
            public readonly Rect[] Rects;
            public readonly uint[] TextureIDs;

            public bool IsDefault => RenderTexture == null;

            public TextureAtlasData(RenderTexture renderTexture, Rect[] rects, uint[] textureIDs)
            {
                RenderTexture = renderTexture;
                Rects = rects;
                TextureIDs = textureIDs;
            }
        }
    }
}