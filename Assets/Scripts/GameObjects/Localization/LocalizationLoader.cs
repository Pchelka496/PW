using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class LocalizationLoader
{
    const string GROUP_LOAD_PATH = "(UnityEngine.AddressableAssets.Addressables.RuntimePath)/Standalon\neWindows64";
    public AssetReference localizationGroup;

    private Dictionary<string, string> currentLocalization = new Dictionary<string, string>();
    

    // public UniTask<Dictionary<string, string>> GetLocalizationDictionary(string language)//format ISO 639-1
    // {
    //     //cvs
    //     //return currentLocalization;
    // }
}
