using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using ScriptableObjects;
using UnityEngine;

namespace GameObjects.Construct
{
    public class ConstructPartsDataLoader
    {
        const string CONSTRUCT_PARTS_LABEL = "ConstructPartData";
        AsyncLazy<ConstructPartData[]> _lazyParts;

        private void CheckAndInitializeLazy()
        {
            if (_lazyParts == null)
            {
                _lazyParts = new AsyncLazy<ConstructPartData[]>(LoadPartsAsync);
            }
        }

        public async UniTask<int> GetAllDataCount()
        {
            CheckAndInitializeLazy();

            var parts = await _lazyParts;
            return parts.Length;
        }

        public async UniTaskVoid LoadAllParts()
        {
            CheckAndInitializeLazy();

            await _lazyParts;
        }

        private async UniTask<ConstructPartData[]> LoadPartsAsync()
        {
            var handle =
                await Additional.AddressableLouderHelper.LoadAssets<ConstructPartData>(
                    (object)CONSTRUCT_PARTS_LABEL);

            var sortedParts = handle.ToArray();

            for (uint i = 0; i < sortedParts.Length; i++)
            {
                sortedParts[i].LoadID = i;
            }

            return sortedParts;
        }

        public async UniTask LoadPartsAsync(Action<ConstructPartData> callback)
        {
            CheckAndInitializeLazy();

            var parts = await _lazyParts;

            foreach (var part in parts)
            {
                callback?.Invoke(part);
            }
        }

        public async UniTask<ConstructPartData[]> GetAllPartsAsync()
        {
            CheckAndInitializeLazy();

            return await _lazyParts;
        }
    }
}