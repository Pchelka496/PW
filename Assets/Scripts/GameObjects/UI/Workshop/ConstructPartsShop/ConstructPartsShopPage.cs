using System;
using System.Collections.Generic;
using System.Linq;
using GameObjects.UI.Workshop.ConstructPartsShop.TextureCreators;
using ScriptableObjects;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace GameObjects.UI.Workshop.ConstructPartsShop
{
    public class ConstructPartsShopPage
    {
        RectTransform _content;
        PageCharacteristics _pageCharacteristics;
        ConstructPartTextureManager _constructPartTextureManager;
        ConstructPartIcon _iconPrefab;

        readonly List<ConstructPartData> _loadedParts = new();

        Dictionary<uint, ConstructPartIcon> _icons = new();

        SortType _currentSortType = SortType.ByName;

        public enum SortType
        {
            ByName,
            ByMass,
            ByCost
        }

        public interface IConstructPartsShopPageInstaller
        {
            public DiContainer DiContainer { get; }
        }

        [Zenject.Inject]
        private void Construct(ConstructPartTextureManager constructPartTextureManager)
        {
            _constructPartTextureManager = constructPartTextureManager;
        }

        public void Initialize(Initializer initializer)
        {
            _content = initializer.Content;
            _pageCharacteristics = initializer.PageCharacteristics;
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

            SortIcons(_currentSortType);
        }

        public void OnAllPartsDataLoaded()
        {
            _constructPartTextureManager.GetTextureAtlas(_loadedParts.ToArray(), OnTextureAtlasLoaded).Forget();
        }

        private void OnTextureLoaded(RenderTexture texture, ConstructPartData data)
        {
            var icon = _icons[data.LoadID];
            icon.IconTexture = texture;
            icon.SetTextureView();
        }

        private void OnTextureAtlasLoaded(RenderTexture texture, Rect[] rects, ConstructPartData[] dataArray)
        {
            for (var i = 0; i < dataArray.Length; i++)
            {
                var icon = _icons[dataArray[i].LoadID];

                icon.IconAtlasTexture = texture;
                icon.AtlasTextureRect = rects[i];
                icon.SetTextureAtlasView();
            }
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
            float offset = _pageCharacteristics.DistanceBetweenIcons;

            float totalIconSize = iconSize + offset;
            float contentWidth = _content.rect.width;

            // Количество колонок, чтобы иконки не выходили за границы
            int columns = Mathf.Max(1, Mathf.FloorToInt((contentWidth - offset) / totalIconSize));

            // Привязываем контент к верхней границе и фиксируем его позицию
            _content.anchorMin = new Vector2(0, 1);
            _content.anchorMax = new Vector2(1, 1);
            _content.pivot = new Vector2(0.5f, 1);
            _content.anchoredPosition = new Vector2(0, 0);

            int index = 0;
            foreach (var icon in sortedIcons)
            {
                int row = index / columns;
                int col = index % columns;

                float posX = (iconSize / 2 + offset) + col * totalIconSize;
                float posY = -((iconSize / 2 + offset) + row * totalIconSize);

                icon.RectTransform.anchorMin = new Vector2(0, 1);
                icon.RectTransform.anchorMax = new Vector2(0, 1);
                icon.RectTransform.pivot = new Vector2(0.5f, 0.5f);
                icon.RectTransform.anchoredPosition = new Vector2(posX, posY);
                icon.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);
                icon.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);

                index++;
            }

            // Финальная высота контейнера
            int totalRows = Mathf.CeilToInt(_icons.Count / (float)columns);
            float totalHeight = totalRows * totalIconSize + offset;

            // Меняем sizeDelta, чтобы высота увеличивалась вниз
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, totalHeight);
        }

        public void ChangeSortType(SortType newSortType) => SortIcons(newSortType);

        public void Open()
        {
            _content.gameObject.SetActive(true);
        }

        public void Close()
        {
            _content.gameObject.SetActive(false);
        }

        private void OnIconClicked(ConstructPartIcon clickedIcon)
        {
            Debug.Log(clickedIcon);
        }

        [System.Serializable]
        public struct Initializer
        {
            [SerializeField] public PageCharacteristics PageCharacteristics;
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