using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameObjects.Construct;
using GameObjects.UI.Workshop.ConstructPartsShop.TextureCreators;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjects.UI.Workshop.ConstructPartsShop
{
    public class ConstructPartsShop : MonoBehaviour
    {
        [HideLabel] [SerializeField] ConstructPartsShopConfig _config;
        [HideLabel] [SerializeField] ScrollRect _scrollRect;
        [SerializeField] PageCreator _pageCreator;

        [SerializeField] bool _isOpen;
        [SerializeField] ButtonAndType[] _switchButtons;

        Dictionary<ConstructPartData.EnumConstructPartType, ConstructPartsShopPage> _pages = new();

        ConstructPartsShopPage _currentPage;
        ConstructPartData.EnumConstructPartType _currentPartsType;

        [Zenject.Inject]
        private void Construct(ConstructPartsDataLoader constructPartsDataLoader,
            ConstructPartTextureManager constructPartTextureManager,
            ConstructPartsShopPage.IConstructPartsShopPageInstaller constructPartsShopPageInstaller)
        {
            _pageCreator.Initialize(constructPartsShopPageInstaller);
            LoadConstructPartsData(constructPartsDataLoader).Forget();
        }

        private async UniTaskVoid LoadConstructPartsData(ConstructPartsDataLoader constructPartsDataLoader)
        {
            constructPartsDataLoader.LoadPartsAsync(OnConstructPartLoaded).Forget();
            await constructPartsDataLoader.GetAllPartsAsync();
            OnAllConstructPartLoaded();
        }

        private void OnConstructPartLoaded(ConstructPartData partData)
        {
            _pages[ConstructPartData.EnumConstructPartType.All].OnPartDataLoaded(partData);
            PartSorting(partData);
        }

        private void PartSorting(ConstructPartData partData)
        {
            _pages[partData.ShopType].OnPartDataLoaded(partData);
        }

        private void OnAllConstructPartLoaded()
        {
            foreach (var page in _pages)
            {
                page.Value.OnAllPartsDataLoaded();
            }
        }

        private void Awake()
        {
            _isOpen = !_isOpen;
            SwitchOpenStatus();
            CreateAllPages();
            InitializeAllButtons();
        }

        public void SwitchOpenStatus() //Button
        {
            _isOpen = !_isOpen;
            gameObject.SetActive(_isOpen);
        }

        private void CreateAllPages()
        {
            var types = (ConstructPartData.EnumConstructPartType[])Enum.GetValues(
                typeof(ConstructPartData.EnumConstructPartType));

            foreach (var type in types)
            {
                _pages.Add(type, _pageCreator.CreateNewPage());

                if (type == _currentPartsType)
                {
                    _currentPage = _pages[type];
                }
            }
        }

        private void InitializeAllButtons()
        {
            for (var i = 0; i < _switchButtons.Length; i++)
            {
                var index = i;
                _switchButtons[i].Button.onClick.AddListener(() => SwitchPage(_switchButtons[index].SwitchType));
            }
        }

        private void SwitchPage(ConstructPartData.EnumConstructPartType partType)
        {
            if (_currentPartsType == partType) return;

            var oldPage = _currentPage;
            _currentPartsType = partType;
            _currentPage = _pages[partType];

            oldPage.Close();
            _currentPage.Open();
        }

        [System.Serializable]
        private struct ButtonAndType
        {
            [SerializeField] public Button Button;
            [SerializeField] public ConstructPartData.EnumConstructPartType SwitchType;
        }

        [System.Serializable]
        private class PageCreator
        {
            [SerializeField] RectTransform _pageContainer;
            [SerializeField] ConstructPartsShopPage.Initializer _initializer;
            ConstructPartsShopPage.IConstructPartsShopPageInstaller _constructPartsShopPageInstaller;

            public void Initialize(
                ConstructPartsShopPage.IConstructPartsShopPageInstaller constructPartsShopPageInstaller)
            {
                _constructPartsShopPageInstaller = constructPartsShopPageInstaller;
            }

            public ConstructPartsShopPage CreateNewPage()
            {
                var newPage =
                    (ConstructPartsShopPage)_constructPartsShopPageInstaller.DiContainer.Instantiate(
                        typeof(ConstructPartsShopPage));

                var initializer = _initializer;
                initializer.Content = CreateNewPageContent();
                newPage.Initialize(initializer);

                return newPage;
            }

            private RectTransform CreateNewPageContent()
            {
                var pageContent = new GameObject("PageContent");

                pageContent.gameObject.SetActive(false);
                pageContent.transform.SetParent(_pageContainer, false);

                var scrollRectTargetImage = pageContent.AddComponent<Image>();

                scrollRectTargetImage.sprite = null;
                scrollRectTargetImage.color = new Color(0, 0, 0, 0);

                var rectTransform = pageContent.GetComponent<RectTransform>();

                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;

                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                return rectTransform;
            }
        }
    }
}