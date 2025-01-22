using Cysharp.Threading.Tasks;
using GameObjects.SceneController.State;
using Interfaces;
using UnityEngine;

namespace GameObjects.SceneController
{
    public static class SceneController
    {
        static ISceneState _currentState;
        static ISceneState _previousState;

        static ISceneState[] _allStates;
        static SceneControllerConfig _config;

        public static void Reset()
        {
            _currentState = null;
            _previousState = null;
            _allStates = null;
        }

        public static void Initialize(SceneControllerConfig config)
        {
            Reset();

            _config = config;

            _allStates = new ISceneState[]
            {
                new OpenWorldState(_config.OpenWorld),
                new CompanyMapState(_config.CompanyMap),
                new CreatingConstructState(_config.CreatingConstruct)
            };
        }

        public static async UniTask PreparationState<T>() where T : ISceneState
        {
            if (!GetNeededState<T>(out var neededState)) return;

            await neededState.OnPreparation(_currentState, _allStates);
        }

        public static async UniTask SetState<T>() where T : ISceneState
        {
            if (!GetNeededState<T>(out var neededState)) return;

            if (_currentState != null)
            {
                await _currentState.OnExit(neededState, _allStates);

                _previousState = _currentState;
            }

            _currentState = neededState;

            await _currentState.OnEnter(_previousState, _allStates);
        }

        private static bool GetNeededState<T>(out ISceneState neededState) where T : ISceneState
        {
            foreach (var state in _allStates)
            {
                if (state is T)
                {
                    neededState = state;
                    return true;
                }
            }

            Debug.LogError($"Failed to find the desired state{typeof(T).Name}");

            neededState = null;
            return false;
        }
    }
}