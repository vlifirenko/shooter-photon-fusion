using UnityEngine;

namespace ShooterPhotonFusion.Utils
{
    public static class Utils
    {
        public static Vector3 GetRandomSpawnPoint() 
            => new Vector3(Random.Range(-20, 20), 4, Random.Range(-20, 20));
    }
}