using Cysharp.Threading.Tasks;
using GameObjects.Construct.Parts;
using ScriptableObjects;
using UnityEngine;

namespace GameObjects.Construct
{
    public class ConstructPartFactory
    {
        ConstructPartsDataLoader _dataLoader;

        [Zenject.Inject]
        private void Construct(ConstructPartsDataLoader dataLoader)
        {
            _dataLoader = dataLoader;
        }

        public async UniTask<ConstructPartCore> GetConstructPart(ConstructPartData data)
        {
            var handle =
                await Additional.AddressableLouderHelper.LoadAssetAsync<GameObject>(data.PrefabReference);

            return handle.Result.GetComponent<ConstructPartCore>();
        }
    }
}