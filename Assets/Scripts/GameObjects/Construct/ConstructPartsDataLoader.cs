using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameObjects.Construct
{
    public class ConstructPartsDataLoader
    {
        const string CONSTRUCT_PARTS_GROUP_NAME = "ConstructPartData";
        AsyncOperationHandle<IList<ScriptableObject>> _partsDataHandle;
        
        private AsyncOperationHandle<IList<ScriptableObject>> LoadConstructParts(Action<ScriptableObject> callback)
        {
            if (_partsDataHandle.Status == AsyncOperationStatus.Succeeded)
            {
                UnloadConstructParts();
                Debug.LogWarning($"Reload construct parts data");
            }

            _partsDataHandle =
                Additional.AddressableLouderHelper.LoadAssets<ScriptableObject>(CONSTRUCT_PARTS_GROUP_NAME, callback);
            
            return _partsDataHandle;
        }

        private void UnloadConstructParts()
        {
            Addressables.Release(_partsDataHandle);
        }
    }
}