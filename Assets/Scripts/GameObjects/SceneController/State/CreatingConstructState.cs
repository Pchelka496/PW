using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace GameObjects.SceneController.State
{
    public class CreatingConstructState : BaseSceneState
    {
        public CreatingConstructState(string sceneAddress) : base(sceneAddress)
        {
        }

        public override async UniTask OnEnter(ISceneState previousState, ISceneState[] allState)
        {
            await base.OnEnter(previousState, allState);

            foreach (var state in allState)
            {
                if (state == this) continue;
                state.UnloadScene();
            }

            EnableAllObjectsInScene();
        }
    }
}