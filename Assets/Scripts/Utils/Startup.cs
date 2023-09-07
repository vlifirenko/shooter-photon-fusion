using UnityEngine;

namespace ShooterPhotonFusion.Utils
{
    public class Startup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InstantiatePrefabs()
        {
            Debug.Log("-- Instantiating objects --");

            var prefabsToInstantiate = Resources.LoadAll<GameObject>("InstantiateOnLoad/");

            foreach (var prefab in prefabsToInstantiate)
            {
                Debug.Log($"Creating {prefab.name}");
                Object.Instantiate(prefab);
            }

            Debug.Log("-- Instantiating objects done --");
        }
    }
}