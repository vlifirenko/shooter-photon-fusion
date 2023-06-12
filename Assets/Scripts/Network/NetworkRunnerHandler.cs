using System;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShooterPhotonFusion.Network
{
    public class NetworkRunnerHandler : MonoBehaviour
    {
        [SerializeField] private NetworkRunner networkRunnerPrefab;

        private NetworkRunner _networkRunner;

        private void Start()
        {
            _networkRunner = Instantiate(networkRunnerPrefab);
            _networkRunner.name = "NetworkRunner";

            var clientTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(),
                SceneManager.GetActiveScene().buildIndex, null);
            
            Debug.Log("Server NetworkRunner started");
        }

        protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene,
            Action<NetworkRunner> initialized)
        {
            var sceneManager = runner.GetComponents(typeof(MonoBehaviour))
                .OfType<INetworkSceneManager>().FirstOrDefault();

            if (sceneManager == null)
                sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

            runner.ProvideInput = true;

            return runner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                Address = address,
                Scene = scene,
                SessionName = "TestRoom",
                Initialized = initialized,
                SceneManager = sceneManager
            });
        }
    }
}