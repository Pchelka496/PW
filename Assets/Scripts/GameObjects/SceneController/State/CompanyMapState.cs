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
                    case CreatingConstructState creatingConstructState:
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

            foreach (var state in _preparedStates)
            {
                if (state == nextState) continue;
                state.UnloadScene();
            }

            _preparedStates.Clear();
        }
    }
}