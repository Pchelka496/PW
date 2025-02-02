using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "newData", menuName = "Scriptable Objects/ConstructPartDataConfig")]
    public class ConstructPartDataConfig : ScriptableObject
    {
        [SerializeField] AssetReference _prefab;
        
        public AssetReference Prefab => _prefab;
    }
}