using System;
using Cysharp.Threading.Tasks;
using GameObjects.SceneController.State;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace GameObjects.SceneController
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Configuration")] 
        [SerializeField] SceneControllerConfig _sceneControllerConfig;

        [Header("Scene loading time")] 
        [SerializeField] float _openWorldScenePreparationTime;
        [SerializeField] float _createConstructScenePreparationTime;
        [SerializeField] float _companyMapScenePreparationTime;

        static SceneLoader _sceneLoader;

        private void Awake()
        {
            if (_sceneLoader == null)
            {
                _sceneLoader = this;
                DontDestroyOnLoad(gameObject);

                InitializeSceneController().Forget();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async UniTask InitializeSceneController()
        {
            var currentScene = SceneManager.GetActiveScene();

            SceneController.Initialize(_sceneControllerConfig);
            await SetCompanyMapScene();

            SceneManager.UnloadSceneAsync(currentScene);
        }

        [MethodButton]
        public void LoadOpenWorldScene()
        {
            var startTime = Time.realtimeSinceStartup;
            SceneController.PreparationState<OpenWorldState>().Forget();
            _openWorldScenePreparationTime = Time.realtimeSinceStartup - startTime;
        }

        [MethodButton]
        public async UniTask SetOpenWorldScene() 
        {
            var startTime = Time.realtimeSinceStartup;
            await SceneController.SetState<OpenWorldState>();
            _openWorldScenePreparationTime = Time.realtimeSinceStartup - startTime;
        }

        [MethodButton]
        public void LoadCreateConstructScene()
        {
            var startTime = Time.realtimeSinceStartup;
            SceneController.PreparationState<CreatingConstructState>().Forget();
            _createConstructScenePreparationTime = Time.realtimeSinceStartup - startTime;
        }

        [MethodButton]
        public async UniTask SetCreateConstructScene() 
        {
            var startTime = Time.realtimeSinceStartup;
            await SceneController.SetState<CreatingConstructState>();
            _createConstructScenePreparationTime = Time.realtimeSinceStartup - startTime;
        }

        [MethodButton]
        public void LoadCompanyMapScene()
        {
            var startTime = Time.realtimeSinceStartup;
            SceneController.PreparationState<CompanyMapState>().Forget();
            _companyMapScenePreparationTime = Time.realtimeSinceStartup - startTime;
        }

        [MethodButton]
        public async UniTask SetCompanyMapScene() 
        {
            var startTime = Time.realtimeSinceStartup;
            await SceneController.SetState<CompanyMapState>();
            _companyMapScenePreparationTime = Time.realtimeSinceStartup - startTime;
        }
    }
}
