using UnityEngine;
using UnityEngine.UI;

namespace GameObjects.UI.CompanyMap
{
    public class FractalToRawImage : MonoBehaviour
    {
        [SerializeField] string _kernelName;
        [SerializeField] ComputeShader _shader;
        [SerializeField] RawImage _debugImage;

        int _kernelIndex;
        uint _threadX;
        RenderTexture _resultTexture;

        private void Awake()
        {
            _resultTexture = new RenderTexture(1920, 1080, 32)
            {
                enableRandomWrite = true
            };
            _resultTexture.Create();

            _debugImage.texture = _resultTexture;

            _kernelIndex = _shader.FindKernel(_kernelName);
            _shader.GetKernelThreadGroupSizes(_kernelIndex, out _threadX, out _, out _);
            _shader.SetVector("Resolution", new Vector2(Screen.width, Screen.height));
            _shader.SetTexture(_kernelIndex, "OutputTexture", _resultTexture);
        }

        private void Update()
        {
            _shader.SetFloat("Time", Time.timeSinceLevelLoad);

            var threadGroups = (int)(Screen.width / _threadX);
            _shader.Dispatch(_kernelIndex, threadGroups, threadGroups, 1);
        }

        private void OnDestroy()
        {
            Destroy(_resultTexture);
        }
    }
}