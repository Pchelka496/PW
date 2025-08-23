using System;
using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Game.Audio;
using Additional;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Random = Unity.Mathematics.Random;

namespace Dmi.Scripts.Audio
{
    public enum AudioType
    {
        Master,
        Music,
        Effects,
        Background
    }

    public class AudioContainer : MonoBehaviour, IDisposable
    {
        [Header("Audio Settings")] [SerializeField]
        AssetReference[] _audioClips;

        [SerializeField] bool _loadOnAwake = false;
        [SerializeField] bool _randomPitch = false;
        [Range(-3f, 3f)] [SerializeField] float _minPitch = 0.8f;
        [Range(-3f, 3f)] [SerializeField] float _maxPitch = 1.2f;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioType _audioType = AudioType.Effects;
        [SerializeField, Range(0f, 2f)] float _localVolumeMultiplier = 1f;
        [SerializeField] bool _multiPlaybackMode = false;

        readonly List<AudioClip> _loadedClips = new();
        readonly List<AsyncOperationHandle<AudioClip>> _handles = new();
        readonly CompositeDisposable _disposable = new();

        static AudioSourcePool _audioSourcePool;
        static Timer _timer;

        int _currentClipIndex = 0;
        bool _loaded = false;
        Random _random;

        float _masterVolume = 1f;
        float _categoryVolume = 1f;
        bool _isMuted = false;

        public bool IsLoaded => _loaded;

        [Inject]
        private void Construct( AudioSourcePool audioSourcePool, Timer timer)
        {
            _audioSourcePool = audioSourcePool;
            _timer = timer;
        }

        private void Awake()
        {
            _random = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue));

            if (_loadOnAwake)
                LoadAll().Forget();
        }

        public async UniTaskVoid LoadAll(CancellationToken cancellationToken = default)
        {
            if (_loaded || _audioClips == null || _audioClips.Length == 0)
                return;

            _loadedClips.Clear();
            _handles.Clear();

            foreach (var assetRef in _audioClips)
            {
                if (assetRef == null)
                    continue;

                var handle = await AddressableLouderHelper.LoadAssetAsync<AudioClip>(assetRef);
                _handles.Add(handle);

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var clip = handle.Result;
                    _loadedClips.Add(clip);

                    if (clip.loadState != AudioDataLoadState.Loaded)
                        clip.LoadAudioData();

                    while (clip.loadState == AudioDataLoadState.Loading)
                        await UniTask.Yield();

                    if (!_multiPlaybackMode && _audioSource != null)
                    {
                        _audioSource.clip = clip;
                        _audioSource.volume = 0f;
                        _audioSource.Play();
                        await UniTask.Delay(50, cancellationToken: cancellationToken);
                        _audioSource.Stop();
                    }
                    else if (_multiPlaybackMode)
                    {
                        var dummy = _audioSourcePool.GetAudioSource();
                        if (dummy != null)
                        {
                            dummy.clip = clip;
                            dummy.volume = 0f;
                            dummy.Play();
                            await UniTask.Delay(50, cancellationToken: cancellationToken);
                            dummy.Stop();
                            _audioSourcePool.ReturnOnPool(dummy);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"[AudioContainer] Failed to load: {assetRef.RuntimeKey}");
                }
            }

            _loaded = true;
        }

        public void UnloadAll()
        {
            foreach (var handle in _handles)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _handles.Clear();
            _loadedClips.Clear();
            _loaded = false;
        }

        public void StopAudio()
        {
            if (_audioSource == null)
                return;

            _audioSource.Stop();
            _audioSource.clip = null;
        }

        public void PlayRandom()
        {
            if (!_loaded || _loadedClips.Count == 0)
            {
                Debug.LogWarning("[AudioContainer] No audio clips loaded.");
                return;
            }

            int index = _random.NextInt(0, _loadedClips.Count);
            PlayClip(_loadedClips[index]);
        }

        public void PlayInOrder()
        {
            if (!_loaded || _loadedClips.Count == 0)
            {
                Debug.LogWarning("[AudioContainer] No audio clips loaded.");
                return;
            }

            var clip = _loadedClips[_currentClipIndex];
            PlayClip(clip);

            _currentClipIndex = (_currentClipIndex + 1) % _loadedClips.Count;
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip == null || _isMuted)
                return;

            if (_multiPlaybackMode)
            {
                var source = _audioSourcePool.GetAudioSource();
                if (source == null)
                    return;

                ApplySettingsToSource(source);

                source.clip = clip;
                source.pitch = _randomPitch
                    ? _random.NextFloat(_minPitch, _maxPitch)
                    : 1f;

                source.volume = _masterVolume * _categoryVolume * _localVolumeMultiplier;
                source.Play();

                float duration = clip.length / source.pitch;

                _timer.AddTimer(duration, () => { _audioSourcePool.ReturnOnPool(source); });
            }
            else
            {
                ApplySettingsToSource(_audioSource);

                _audioSource.clip = clip;
                _audioSource.pitch = _randomPitch
                    ? _random.NextFloat(_minPitch, _maxPitch)
                    : 1f;

                UpdateVolume();
                _audioSource.Play();
            }
        }

        private void ApplySettingsToSource(AudioSource source)
        {
            if (source == null) return;

            source.transform.position = transform.position;

            source.spatialBlend = _audioSource.spatialBlend;
            source.minDistance = _audioSource.minDistance;
            source.maxDistance = _audioSource.maxDistance;
            source.rolloffMode = _audioSource.rolloffMode;
            source.dopplerLevel = _audioSource.dopplerLevel;
            source.spread = _audioSource.spread;
            source.priority = _audioSource.priority;
            source.outputAudioMixerGroup = _audioSource.outputAudioMixerGroup;
        }

        private void UpdateVolume()
        {
            if (_audioSource != null)
            {
                _audioSource.volume = _masterVolume * _categoryVolume * _localVolumeMultiplier;
            }
        }

        public float GetClipLength()
        {
            if (_audioSource != null && _audioSource.clip != null)
                return _audioSource.clip.length;

            return 0f;
        }

        private void OnDestroy()
        {
            if (_loaded)
                UnloadAll();

            Dispose();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}