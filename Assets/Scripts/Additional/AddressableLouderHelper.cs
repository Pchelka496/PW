using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Additional
{
    public static class AddressableLouderHelper
    {
        public static async UniTask<AsyncOperationHandle<T>> LoadAssetAsync<T>(
            AssetReference reference)
        {
            if (reference == null)
            {
                Debug.LogError("AssetReference is null");
                return default;
            }

            var handle = Addressables.LoadAssetAsync<T>(reference);

            await handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle;
            }
            else
            {
                Debug.LogError($"Error loading via Addressable. GUID - {reference.AssetGUID}");
                return default;
            }
        }

        public static async UniTask<AsyncOperationHandle<T>> LoadAssetAsync<T>(
            string address)
        {
            if (address == null)
            {
                Debug.LogError("Address is null");
                return default;
            }

            var handle = Addressables.LoadAssetAsync<T>(address);

            await handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle;
            }
            else
            {
                Debug.LogError($"Error loading via Addressable. Asset Address - {address}");
                return default;
            }
        }

        public static async UniTask<AsyncOperationHandle<IList<T>>> LoadAssetsAsync<T>(
            string address)
        {
            if (address == null)
            {
                Debug.LogError("Address is null");
                return default;
            }

            var handle = Addressables.LoadAssetsAsync<T>(address);

            await handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle;
            }
            else
            {
                Debug.LogError($"Error loading via Addressable. Asset Address - {address}");
                return default;
            }
        }

        public static AsyncOperationHandle<IList<T>> LoadAssets<T>(string address, Action<T> callback)
        {
            if (address == null)
            {
                Debug.LogError("Address is null");
                return default;
            }

            var handle = Addressables.LoadAssetsAsync<T>(address, callback);

            return handle;
        }
    }
}