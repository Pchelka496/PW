using System.Collections.Generic;
using Additional;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace GameObjects.SceneController.State
{
    public abstract class BaseSceneState : ISceneState, System.IDisposable
    {
        readonly string _sceneAddress;
        SceneInstance _sceneInstance;
        bool _sceneObjectActiveStatus = true;

        readonly List<Camera> _sceneCameras = new();
        AudioListener _sceneAudioListeners;

        private bool IsValid => _sceneInstance.Scene.isLoaded && _sceneInstance.Scene.IsValid();

        protected BaseSceneState(string sceneAddress)
        {
            _sceneAddress = sceneAddress;
        }

        public virtual async UniTask OnPreparation(ISceneState currentState, ISceneState[] allState)
        {
            if (IsValid) return;

            try
            {
                var loadHandle = Addressables.LoadSceneAsync(_sceneAddress, LoadSceneMode.Additive);
                _sceneInstance = await loadHandle.Task;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load scene {_sceneAddress}: {ex.Message}");
                return;
            }

            UpdateCameras();
            UpdateAudioListeners();
        }

        public virtual async UniTask OnEnter(ISceneState previousState, ISceneState[] allState)
        {
            if (SetActiveScene(_sceneInstance.Scene)) return;

            await OnPreparation(previousState, allState);

            if (!SetActiveScene(_sceneInstance.Scene))
            {
                Debug.LogError($"Scene {_sceneInstance.Scene} is not active");
            }
        }

        public virtual UniTask OnExit(ISceneState currentState, ISceneState[] allState)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask UnloadScene()
        {
            if (!IsValid) return;

            var unloadHandle = Addressables.UnloadSceneAsync(_sceneInstance);

            await UniTask.WaitForEndOfFrame();

            await unloadHandle.ToUniTask();

            _sceneCameras.Clear();
            _sceneAudioListeners = null;
        }

        public void EnableAllObjectsInScene()
        {
            if (_sceneObjectActiveStatus) return;
            _sceneObjectActiveStatus = true;

            _sceneInstance.Scene.EnableAllObjectsInScene();
        }

        public void DisableAllObjectInScene()
        {
            if (!_sceneObjectActiveStatus) return;
            _sceneObjectActiveStatus = false;

            _sceneInstance.Scene.DisableAllObjectsInScene();
        }

        public void EnableAudioListeners()
        {
            if (_sceneAudioListeners == null) return;
            _sceneAudioListeners.enabled = true;
        }

        public void DisableAudioListeners()
        {
            if (_sceneAudioListeners == null) return;
            _sceneAudioListeners.enabled = false;
        }

        public void EnableCameras()
        {
            foreach (var cameras in _sceneCameras)
            {
                cameras.enabled = true;
            }
        }

        public void DisableCameras()
        {
            foreach (var cameras in _sceneCameras)
            {
                cameras.enabled = false;
            }
        }

        protected virtual bool SetActiveScene(Scene scene)
        {
            if (IsValid)
            {
                SceneManager.SetActiveScene(scene);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void DisableOtherCameras(LinkedList<ISceneState> anyState)
        {
            foreach (var state in anyState)
            {
                state.DisableCameras();
            }
        }

        protected void DisableOtherAudioListener(LinkedList<ISceneState> anyState)
        {
            foreach (var state in anyState)
            {
                state.DisableAudioListeners();
            }
        }

        private void UpdateCameras()
        {
            _sceneCameras.Clear();

            _sceneInstance.Scene.FillListScenesCameras(_sceneCameras);
        }

        private void UpdateAudioListeners()
        {
            _sceneInstance.Scene.SetAudioListeners(out _sceneAudioListeners, _sceneCameras);
        }

        public void Dispose()
        {
            if (IsValid)
            {
                UnloadScene().Forget();
            }
        }
    }
}