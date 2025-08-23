using Dmi.Scripts.Audio;
using UnityEngine;

namespace _Project.Scripts.Game.Audio
{
    [RequireComponent(typeof(Collider))]
    public class PlayAudioOnTrigger : MonoBehaviour
    {
        [SerializeField] AudioContainer _audioContainer;

        private void OnTriggerEnter(Collider other)
        {
            if (_audioContainer == null)
            {
                Debug.LogWarning("[PlayAudioOnTrigger] AudioContainer is not assigned.", this);
                return;
            }

            _audioContainer.PlayInOrder();
        }
    }
}