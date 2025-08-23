using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;

namespace Dmi.Scripts.Audio
{
    public class AudioSourcePool : IDisposable
    {
        const int INITIAL_POOL_SIZE = 5;

        readonly AudioSource _audioSourcePrefab;
        readonly Transform _transform;
        readonly List<AudioSource> _sourcesPool = new();
        readonly List<AudioSource> _allSources = new();

        IDisposable _volumeSubscription;
        Timer _timer;

        public AudioSourcePool(Transform transform)
        {
            _transform = transform;

            _audioSourcePrefab = CreateAudioSourcePrefab();
            RegisterSource(_audioSourcePrefab);

            for (int i = 0; i < INITIAL_POOL_SIZE; i++)
                RegisterSource(CreateNewAudioSource());
        }

        [Inject]
        private void Construct(Timer timer)
        {
            _timer = timer;
        }

        private void UpdateAllSourceVolumes(float newVolume)
        {
            float clampedVolume = Mathf.Clamp01(newVolume);
            foreach (var source in _allSources)
            {
                if (source.isPlaying)
                    source.volume = clampedVolume;
            }
        }

        private AudioSource CreateAudioSourcePrefab()
        {
            var go = UnityEngine.Object.Instantiate(new GameObject("AudioSource"), _transform);
            go.hideFlags = HideFlags.HideAndDontSave;
            return go.AddComponent<AudioSource>();
        }

        private AudioSource CreateNewAudioSource()
        {
            var source = UnityEngine.Object.Instantiate(_audioSourcePrefab, _transform);
            source.gameObject.hideFlags = HideFlags.HideAndDontSave;
            return source;
        }

        private void RegisterSource(AudioSource source)
        {
            _sourcesPool.Add(source);
            _allSources.Add(source);
        }

        public AudioSource GetAudioSource()
        {
            if (_sourcesPool.Count > 0)
            {
                int lastIndex = _sourcesPool.Count - 1;
                var source = _sourcesPool[lastIndex];
                _sourcesPool.RemoveAt(lastIndex);
                return source;
            }

            var newSource = CreateNewAudioSource();
            _allSources.Add(newSource);
            return newSource;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReturnOnPool(AudioSource source)
        {
            if (source == null) return;

            source.Stop();
            source.clip = null;
            _sourcesPool.Add(source);
        }

        public void Dispose()
        {
            _volumeSubscription?.Dispose();
            _volumeSubscription = null;

            foreach (var source in _allSources)
            {
                if (source != null)
                    UnityEngine.Object.Destroy(source.gameObject);
            }

            _allSources.Clear();
            _sourcesPool.Clear();
        }
    }
}