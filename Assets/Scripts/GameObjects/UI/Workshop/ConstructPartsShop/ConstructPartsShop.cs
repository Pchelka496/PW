using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameObjects.Construct;
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

        Dictionary<ConstructPartData.ConstructPartType, ConstructPartsShopPage> _pages = new();
        ConstructPartsShopPage _allParts;

        ConstructPartsShopPage _currentPage;
        
        [Zenject.Inject]
        private void Construct(ConstructPartsDataLoader constructPartsDataLoader)
        {
            _allParts = _pageCreator.CreateNewPage();

            constructPartsDataLoader.LoadPartsAsync(OnConstructPartLoaded).Forget();
        }

        private void OnConstructPartLoaded(ConstructPartData partData)
        {
            _allParts.OnPartDataLoaded(partData);

            PartSorting(partData);
        }

        private void PartSorting(ConstructPartData partData)
        {
            if (!_pages.ContainsKey(partData.PartType))
            {
                _pages.Add(partData.PartType, _pageCreator.CreateNewPage());
            }

            _pages[partData.PartType].OnPartDataLoaded(partData);
        }
        
        [System.Serializable]
        private class PageCreator
        {
            [SerializeField] RectTransform _pageContainer;
            [SerializeField] ConstructPartsShopPage.Initializer _initializer;

            public ConstructPartsShopPage CreateNewPage()
            {
                var newPage = new ConstructPartsShopPage();

                var initializer = _initializer;
                initializer.Content = CreateNewPageContent();
                newPage.Initialize(initializer);

                return newPage;
            }

            private RectTransform CreateNewPageContent()
            {
                var pageContent = Instantiate(new GameObject("PageContent"), _pageContainer.transform);

                var rectTransform = pageContent.AddComponent<RectTransform>();

                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;

                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                return rectTransform;
            }
        }
    }
}