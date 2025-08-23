using UnityEngine;
using Zenject;

namespace Dmi.Scripts.UI
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] InteractIndicator _interactIndicator;
        
        public override void InstallBindings()
        {
            Container.Bind<IInteractIndicator>().FromInstance(_interactIndicator).NonLazy();
        }
    }
}