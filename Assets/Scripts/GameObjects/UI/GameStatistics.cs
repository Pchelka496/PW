using Nenn.InspectorEnhancements.Runtime.Attributes;
using TMPro;
using UnityEngine;

namespace GameObjects.UI
{
    public class GameStatistics : MonoBehaviour
    {
        [Required] [SerializeField] TextMeshProUGUI _averageFpsTextMeshPro;
        [SerializeField] int _frameRange = 60;

        float _deltaTime;
        float _totalDeltaTime;
        int _frameCount;

        int[] _fpsBuffer;
        int _fpsBufferIndex;

        public int AverageFPS { get; private set; }
        public int HighestPfs { get; private set; }
        public int LowersFPS { get; private set; }

        static GameStatistics _gameStatistics;

        private void Awake()
        {
            transform.SetParent(null);

            if (_gameStatistics == null)
            {
                _gameStatistics = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (_fpsBuffer == null || _frameRange != _fpsBuffer.Length)
            {
                InitializeBuffer();
            }

            UpdateBuffer();
            CalculateFps();

            _averageFpsTextMeshPro.text = $"AverageFPS: {AverageFPS}";
        }

        private void InitializeBuffer()
        {
            if (_frameRange <= 0)
            {
                _frameRange = 1;
            }

            _fpsBuffer = new int[_frameRange];
            _fpsBufferIndex = 0;
        }

        private void UpdateBuffer()
        {
            _fpsBuffer[_fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
            if (_fpsBufferIndex >= _frameRange)
            {
                _fpsBufferIndex = 0;
            }
        }

        private void CalculateFps()
        {
            int sum = 0;
            int lowest = int.MaxValue;
            int highest = 0;
            for (int i = 0; i < _frameRange; i++)
            {
                int fps = _fpsBuffer[i];
                sum += fps;
                if (fps > highest)
                {
                    highest = fps;
                }

                if (fps < lowest)
                {
                    lowest = fps;
                }
            }

            HighestPfs = highest;
            LowersFPS = lowest;
            AverageFPS = sum / _frameRange;
        }

        private void Reset()
        {
            _averageFpsTextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        }
    }
}