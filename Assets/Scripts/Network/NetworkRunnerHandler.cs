using System;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using ShooterPhotonFusion.Movement;
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

            var clientTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient,
                GameManager.Instance.GetConnectionToken(), NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

            Debug.Log("Server NetworkRunner started");
        }

        public void StartHostMigration(HostMigrationToken hostMigrationToken)
        {
            _networkRunner = Instantiate(networkRunnerPrefab);
            _networkRunner.name = "NetworkRunner - Migrated";

            var clientTask = InitializeNetworkRunnerHostMigration(_networkRunner, hostMigrationToken);

            Debug.Log("Host migration started");
        }

        private INetworkSceneManager GetSceneManager(NetworkRunner runner)
        {
            var sceneManager = runner.GetComponents(typeof(MonoBehaviour))
                .OfType<INetworkSceneManager>().FirstOrDefault();

            if (sceneManager == null)
                sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

            return sceneManager;
        }

        protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, byte[] connectionToken,
            NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
        {
            var sceneManager = GetSceneManager(runner);

            runner.ProvideInput = true;

            return runner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                Address = address,
                Scene = scene,
                SessionName = "TestRoom",
                Initialized = initialized,
                SceneManager = sceneManager,
                ConnectionToken = connectionToken
            });
        }


        protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            var sceneManager = GetSceneManager(runner);

            runner.ProvideInput = true;

            return runner.StartGame(new StartGameArgs
            {
                //GameMode = gameMode,
                //Address = address,
                //Scene = scene,
                //SessionName = "TestRoom",
                //Initialized = initialized,
                SceneManager = sceneManager,
                HostMigrationToken = hostMigrationToken,
                HostMigrationResume = HostMigrationResume
            });
        }

        private void HostMigrationResume(NetworkRunner runner)
        {
            Debug.Log("HostMigrationResume started");

            foreach (var resumeNetworkObject in runner.GetResumeSnapshotNetworkObjects())
            {
                if (resumeNetworkObject.TryGetBehaviour<NetworkCharacterControllerPrototypeCustom>(out var characterController))
                {
                    runner.Spawn(resumeNetworkObject,
                        characterController.ReadPosition(),
                        characterController.ReadRotation(),
                        onBeforeSpawned: (_, newNetworkObject) => newNetworkObject.CopyStateFrom(resumeNetworkObject));
                }
            }

            Debug.Log("HostMigrationResume completed");
        }
    }
}