using Dmi.Scripts.Audio;
using UnityEngine;

namespace _Project.Scripts.Game.Audio
{
    public class UIAudioModule : MonoBehaviour
    {
        [Header("UI Audio Containers")] [SerializeField]
        AudioContainer _selectAudio;

        [SerializeField] AudioContainer _pressAudio;
        [SerializeField] AudioContainer _undoAudio;

        public void PlaySelect()
        {
            _selectAudio?.PlayInOrder();
        }

        public void PlayPress()
        {
            _pressAudio?.PlayInOrder();
        }

        public void PlayUndo()
        {
            _undoAudio?.PlayInOrder();
        }
        
        public void PreloadAll()
        {
            _selectAudio?.LoadAll().Forget();
            _pressAudio?.LoadAll().Forget();
            _undoAudio?.LoadAll().Forget();
        }
        
        public void StopAll()
        {
            _selectAudio?.StopAudio();
            _pressAudio?.StopAudio();
            _undoAudio?.StopAudio();
        }
    }
}