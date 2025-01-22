using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SceneControllerConfig", menuName = "Scriptable Objects/SceneControllerConfig")]
public class SceneControllerConfig : ScriptableObject
{
    [SerializeField] AssetReference _openWorld;
    [SerializeField] AssetReference _companyMap;
    [SerializeField] AssetReference _creatingConstruct;
    
    public string OpenWorld => _openWorld.AssetGUID;
    public string CompanyMap => _companyMap.AssetGUID;
    public string CreatingConstruct => _creatingConstruct.AssetGUID;
    
}