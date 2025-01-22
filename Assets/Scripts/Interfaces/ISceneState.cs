using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Interfaces
{
    public interface ISceneState
    {
        UniTask OnPreparation(ISceneState currentState, ISceneState[] allState);
        UniTask OnEnter(ISceneState previousState, ISceneState[] allState);
        UniTask OnExit(ISceneState nextState, ISceneState[] allState);

        UniTask UnloadScene();

        void EnableAllObjectsInScene();
        void DisableAllObjectInScene();

        void EnableAudioListeners();
        void DisableAudioListeners();

        void EnableCameras();
        void DisableCameras();
    }
}