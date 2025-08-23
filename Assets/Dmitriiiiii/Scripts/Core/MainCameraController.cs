using UnityEngine;

namespace Dmi.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class MainCameraController : MonoBehaviour
    {
        [SerializeField] Camera _camera;
   
        public Camera Camera => _camera;

        [Zenject.Inject]
        private void Construct()
        {
            
        }
    }
}