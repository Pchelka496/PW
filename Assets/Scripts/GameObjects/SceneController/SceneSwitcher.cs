using Cysharp.Threading.Tasks;
using GameObjects.SceneController.State;
using UnityEngine;

namespace GameObjects.SceneController
{
    public class SceneSwitcher : MonoBehaviour
    {
        public void LoadOpenWorldScene()
            => SceneController.PreparationState<OpenWorldState>().Forget();

        public void SetOpenWorldScene()
            => SceneController.SetState<OpenWorldState>().Forget();


        public void LoadCreateConstructScene()
            => SceneController.PreparationState<WorkshopState>().Forget();
    
        public void SetCreateConstructScene()
            => SceneController.SetState<WorkshopState>().Forget();


        public void LoadCompanyMapScene()
            => SceneController.PreparationState<CompanyMapState>().Forget();

        public void SetCompanyMapScene()
            => SceneController.SetState<CompanyMapState>().Forget();
    }
}