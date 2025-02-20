using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameObjects.UI.Workshop.ConstructPartsShop.TextureCreators;
using ScriptableObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameObjects.UI.Workshop.ConstructPartsShop
{
    public class ConstructPartsShopPage
    {
        RectTransform _content;
        PageCharacteristics _pageCharacteristics;
        ConstructPartTextureManager _constructPartTextureManager;
        ConstructPartIcon _iconPrefab;

        // readonly List<ConstructPartIcon> _icons = new();
        readonly List<ConstructPartData> _loadedParts = new();

        Dictionary<uint, ConstructPartIcon> _icons = new();

        SortType _currentSortType = SortType.ByName;

        public enum SortType
        {
            ByName,
            ByMass,
            ByCost
        }

        public void Initialize(Initializer initializer)
        {
            _content = initializer.Content;
            _pageCharacteristics = initializer.PageCharacteristics;
            _constructPartTextureManager = initializer.ConstructPartTextureManager;
            _iconPrefab = initializer.IconPrefab;

            _icons.Clear();
            _loadedParts.Clear();
        }

        public void OnPartDataLoaded(ConstructPartData data)
        {
            _loadedParts.Add(data);

            if (!_icons.ContainsKey(data.LoadID))
            {
                _icons.Add(data.LoadID, CreateIcon());
            }

            var icon = _icons[data.LoadID];
            icon.ConstructPartData = data;
            _constructPartTextureManager.GetTexture(data, OnTextureLoaded).Forget();
            _constructPartTextureManager.GetTextureAtlas(_loadedParts.ToArray(), OnTextureAtlasLoaded).Forget();

            SortIcons(_currentSortType);
        }

        private void OnTextureLoaded(RenderTexture texture, ConstructPartData data)
        {
            var icon = _icons[data.LoadID];
            icon.Image.texture = texture;
            icon.ConstructTexture = texture;
        }

        private void OnTextureAtlasLoaded(RenderTexture texture, Rect[] rects, ConstructPartData[] datas)
        {
        }

        private ConstructPartIcon CreateIcon()
        {
            var icon = Object.Instantiate(_iconPrefab, _content);

            icon.Button.onClick.AddListener(() => OnIconClicked(icon));

            SortIcons(_currentSortType);

            return icon;
        }

        private void SortIcons(SortType sortType)
        {
            _currentSortType = sortType;
            var iconsList = _icons.Values.ToList();

            IEnumerable<ConstructPartIcon> sortedIcons = sortType switch
            {
                SortType.ByName => iconsList.OrderBy(icon => _loadedParts[iconsList.IndexOf(icon)].PartName),
                SortType.ByMass => iconsList.OrderBy(icon => _loadedParts[iconsList.IndexOf(icon)].Mass),
                SortType.ByCost => iconsList.OrderBy(icon => _loadedParts[iconsList.IndexOf(icon)].Cost),
                _ => iconsList
            };

            float iconSize = _pageCharacteristics.IconSize;
            float spacingBetweenIcons = _pageCharacteristics.DistanceBetweenIcons;
            float verticalSpacing = _pageCharacteristics.VerticalSpacing;
            float horizontalSpacing = _pageCharacteristics.HorizontalSpacing;

            float totalIconWidth = iconSize + horizontalSpacing;
            float totalIconHeight = iconSize + verticalSpacing;

            int columns = Mathf.Max(1, Mathf.FloorToInt((_content.rect.width + spacingBetweenIcons) / totalIconWidth));

            float startX = -_content.rect.width / 2 + horizontalSpacing / 2;
            float startY = _content.rect.height / 2 - verticalSpacing / 2;

            int index = 0;
            foreach (var icon in sortedIcons)
            {
                int row = index / columns;
                int col = index % columns;

                float posX = startX + col * totalIconWidth;
                float posY = startY - row * totalIconHeight;

                icon.RectTransform.anchoredPosition = new Vector2(posX, posY);
                icon.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);
                icon.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);

                index++;
            }

            float totalHeight = Mathf.CeilToInt(_icons.Count / (float)columns) * totalIconHeight;
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        }

        public void ChangeSortType(SortType newSortType) => SortIcons(newSortType);

        private void OnIconClicked(ConstructPartIcon clickedIcon)
        {
            Debug.Log(clickedIcon);
        }

        [System.Serializable]
        public struct Initializer
        {
            [SerializeField] public PageCharacteristics PageCharacteristics;
            [SerializeField] public ConstructPartTextureManager ConstructPartTextureManager;
            [SerializeField] public CameraTextureAtlasCreator CameraTextureAtlasCreator;
            [SerializeField] public ConstructPartIcon IconPrefab;

            [NonSerialized] public RectTransform Content;
            
        }

        [System.Serializable]
        public struct PageCharacteristics
        {
            [SerializeField] float _iconSize;
            [SerializeField] float _distanceBetweenIcons;

            [Space] [SerializeField] float _verticalSpacing;
            [SerializeField] float _horizontalSpacing;

            public float IconSize => _iconSize;
            public float DistanceBetweenIcons => _distanceBetweenIcons;

            public float VerticalSpacing => _verticalSpacing;
            public float HorizontalSpacing => _horizontalSpacing;
        }
    }
}