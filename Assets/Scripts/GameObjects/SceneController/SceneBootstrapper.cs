using Cysharp.Threading.Tasks;
using GameObjects.SceneController.State;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameObjects.SceneController
{
    public class SceneBootstrapper : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        SceneControllerConfig _sceneControllerConfig;

        static SceneBootstrapper _sceneBootstrapper;

        private void Awake()
        {
            transform.parent = null;

            if (_sceneBootstrapper == null)
            {
                _sceneBootstrapper = this;
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
            var scene = SceneManager.GetActiveScene();

            SceneController.Initialize(_sceneControllerConfig);
            await SetCompanyMapScene();

            await UniTask.WaitForEndOfFrame();

            SceneManager.UnloadSceneAsync(scene).ToUniTask().Forget();
        }

//#if UNITY_EDITOR
        [Header("Scene loading time")] [SerializeField]
        // ReSharper disable once NotAccessedField.Local
        float _openWorldScenePreparationTime;

        // ReSharper disable once NotAccessedField.Local
        [SerializeField] float _createConstructScenePreparationTime;

        // ReSharper disable once NotAccessedField.Local
        [SerializeField] float _companyMapScenePreparationTime;

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
            SceneController.PreparationState<WorkshopState>().Forget();
            _createConstructScenePreparationTime = Time.realtimeSinceStartup - startTime;
        }

        [MethodButton]
        public async UniTask SetCreateConstructScene()
        {
            var startTime = Time.realtimeSinceStartup;
            await SceneController.SetState<WorkshopState>();
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
//#endif
    }
}