using Fusion;
using ShooterPhotonFusion.Ui;
using UnityEngine;

namespace ShooterPhotonFusion.Network
{
    public class NetworkInGameMessages : NetworkBehaviour
    {
        private InGameMessageUiHandler _inGameMessageUiHandler;

        public void SendInGameRPCMessage(string userNickName, string message)
        {
            RPC_InGameMessage($"<b>{userNickName}</b> {message}");
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_InGameMessage(string message, RpcInfo info = default)
        {
            Debug.Log($"[RPC] InGameMessage {message}");

            if (_inGameMessageUiHandler == null)
                _inGameMessageUiHandler = NetworkPlayer.Local.localCameraHandler.GetComponentInChildren<InGameMessageUiHandler>();

            if (_inGameMessageUiHandler != null)
                _inGameMessageUiHandler.OnGameMessageReceived(message);
        }
    }
}