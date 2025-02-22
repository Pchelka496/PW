using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "newData", menuName = "Scriptable Objects/ConstructPartData")]
    public class ConstructPartData : ScriptableObject
    {
        [Space] [SerializeField] AssetReference _prefabReference;
        [Space] [SerializeField] string _partName;
        [Range(0, 100000)] [SerializeField] int _mass;
        [SerializeField] EnumConstructPartType _shopType;

        [Header("Construct part shop")] //
        [Range(0, 100000), SerializeField]
        int _cost;

        [Space] [SerializeField] Vector3 _localPositionForRenderTexture;
        [SerializeField] Quaternion _rotationForRenderTexture;

        public AssetReference PrefabReference => _prefabReference;
        public string PartName => _partName;
        public int Mass => _mass;
        public EnumConstructPartType ShopType => _shopType;

        public int Cost => _cost;
        public Vector3 LocalPositionForRenderTexture => _localPositionForRenderTexture;
        public Quaternion RotationForRenderTexture => _rotationForRenderTexture;

        public uint LoadID { get; set; }

        public enum EnumConstructPartType
        {
            CommandModules,
            Engines,
            Aerodynamics,
            All = CommandModules | Engines | Aerodynamics
        }
    }
}