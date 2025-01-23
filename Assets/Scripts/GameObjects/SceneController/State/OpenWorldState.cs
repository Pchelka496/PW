using Cysharp.Threading.Tasks;
using Interfaces;

namespace GameObjects.SceneController.State
{
    public class OpenWorldState : BaseSceneState
    {
        public OpenWorldState(string sceneAddress) : base(sceneAddress)
        {
        }

        public override async UniTask OnEnter(ISceneState previousState, ISceneState[] allState)
        {
            await base.OnEnter(previousState, allState);

            foreach (var state in allState)
            {
                if (state == this) continue;
                await state.UnloadScene();
            }

            EnableAllObjectsInScene();
        }
    }
}