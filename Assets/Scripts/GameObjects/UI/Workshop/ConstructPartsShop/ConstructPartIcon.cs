using Nenn.InspectorEnhancements.Runtime.Attributes;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjects.UI.Workshop.ConstructPartsShop
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(Button))]
    public class ConstructPartIcon : MonoBehaviour
    {
        [HideLabel] [SerializeField] Button _button;
        [HideLabel] [SerializeField] RawImage _image;
        [HideLabel] [SerializeField] RectTransform _rectTransform;

        public RawImage Image => _image;
        public Button Button => _button;
        public RectTransform RectTransform => _rectTransform;

        public ConstructPartData ConstructPartData { get; set; }

        public RenderTexture ConstructTexture { get; set; }

        public RenderTexture ConstructAtlasTexture { get; set; }
        public Rect AtlasTextureRect { get; set; }
        
        private void Reset()
        {
            _button = gameObject.GetComponent<Button>();
            _image = gameObject.GetComponent<RawImage>();
            _rectTransform = gameObject.GetComponent<RectTransform>();
        }
    }
}