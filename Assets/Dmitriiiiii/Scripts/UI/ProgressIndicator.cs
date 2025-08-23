using UnityEngine;
using UnityEngine.UI;

namespace Dmi.Scripts.UI
{
    [RequireComponent(typeof(Image))]
    public class ProgressIndicator : MonoBehaviour
    {
        [SerializeField] Image _progressImage;

        private void Awake()
        {
            SetProgress(0f);
        }

        public void SetProgress(float value01)
        {
            _progressImage.fillAmount = Mathf.Clamp01(value01);
        }

        private void Reset()
        {
            _progressImage = GetComponent<Image>();
        }
    }
}