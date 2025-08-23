using Dmi.Scripts.Audio;
using UnityEngine;

namespace _Project.Scripts.Game.Audio
{
    public class TutorialAudioModule : MonoBehaviour
    {
        [Header("Tutorial Audio Containers")]
        [SerializeField] AudioContainer _taskPartCompletedAudio;
        [SerializeField] AudioContainer _taskCompletedAudio;
        [SerializeField] AudioContainer _newTasksAppearedAudio;

        public void PlayTaskPartCompleted()
        {
            _taskPartCompletedAudio.PlayRandom();
        }

        public void PlayTaskCompleted()
        {
            _taskCompletedAudio.PlayRandom();
        }

        public void PlayNewTasksAppeared()
        {
            _newTasksAppearedAudio.PlayRandom();
        }
    }
}