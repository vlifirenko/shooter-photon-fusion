﻿using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using ShooterPhotonFusion.Input;
using UnityEngine;

namespace ShooterPhotonFusion.Network
{
    public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkPlayer playerPrefab;

        private CharacterInputHandler _characterInputHandler;
        
        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer");
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Debug.Log("OnPlayerJoined we are server. Spawning player");
                runner.Spawn(playerPrefab, Utils.Utils.GetRandomSpawnPoint(), Quaternion.identity, player);
            }
            else
                Debug.Log("OnPlayerJoined");
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_characterInputHandler == null && NetworkPlayer.Local != null)
                _characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();

            if (_characterInputHandler != null)
                input.Set(_characterInputHandler.GetNetworkInput());
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("OnShutdown");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log("OnDisconnectedFromServer");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("OnConnectRequest");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log("OnConnectFailed");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log("OnHostMigration");

            await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

            FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}