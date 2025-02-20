using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Interfaces;

namespace GameObjects.SceneController.State
{
    public class CompanyMapState : BaseSceneState
    {
        readonly List<ISceneState> _preparedStates = new();

        public CompanyMapState(string sceneAddress) : base(sceneAddress)
        {
        }

        public override async UniTask OnEnter(ISceneState previousState, ISceneState[] allState)
        {
            await base.OnEnter(previousState, allState);
            //PreparationAnyState(allState).Forget();
        }

        private async UniTask PreparationAnyState(ISceneState[] allState)
        {
            foreach (var state in allState)
            {
                switch (state)
                {
                    case OpenWorldState openWorldState:
                    {
                        await openWorldState.OnPreparation(this, allState);
                        _preparedStates.Add(openWorldState);

                        break;
                    }
                    case WorkshopState creatingConstructState:
                    {
                        await creatingConstructState.OnPreparation(this, allState);
                        _preparedStates.Add(creatingConstructState);

                        break;
                    }
                }
            }

            await UniTask.WaitForEndOfFrame();

            foreach (var state in allState)
            {
                if (state == this) continue;
                state.DisableAllObjectInScene();
            }
        }

        public override async UniTask OnExit(ISceneState nextState, ISceneState[] allState)
        {
            await base.OnExit(nextState, allState);

            var preparedStatesCopy = new List<ISceneState>(_preparedStates);
            _preparedStates.Clear();

            foreach (var state in preparedStatesCopy)
            {
                state.UnloadScene().Forget();
            }
        }
    }
}