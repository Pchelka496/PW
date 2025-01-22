using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Additional
{
    public static class SceneExpansion
    {
        public static void DisableAllObjectsInScene(this Scene scene)
        {
            if (!scene.isLoaded)
            {
                Debug.LogWarning($"Scene {scene.name} is not loaded.");
                return;
            }

            SetRootObjectActiveStatus(scene.GetRootGameObjects(), false);
        }

        public static void EnableAllObjectsInScene(this Scene scene)
        {
            if (!scene.isLoaded)
            {
                Debug.LogWarning($"Scene {scene.name} is not loaded.");
                return;
            }

            SetRootObjectActiveStatus(scene.GetRootGameObjects(), true);
        }

        public static void FillListScenesCameras(this Scene scene, List<Camera> sceneCameras)
        {
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                sceneCameras.AddRange(rootObject.GetComponentsInChildren<Camera>(true));
            }
        }

        public static bool SetAudioListeners(this Scene scene,
            out AudioListener audioListeners,
            List<Camera> sceneCameras = null)
        {
            if (sceneCameras != null)
            {
                foreach (var camera in sceneCameras)
                {
                    audioListeners = camera.GetComponent<AudioListener>();
                    if (audioListeners != null) return true;
                }
            }
            else
            {
                var rootObjects = scene.GetRootGameObjects();
                
                foreach (var rootObject in rootObjects)
                {
                    audioListeners = rootObject.GetComponentInChildren<AudioListener>(true);
                    if (audioListeners != null) return true;
                }
            }
            
            audioListeners = null;
            return false;
        }

        private static void SetRootObjectActiveStatus(GameObject[] rootObjects, bool flag)
        {
            foreach (var rootObject in rootObjects)
            {
                rootObject.SetActive(flag);
            }
        }
    }
}