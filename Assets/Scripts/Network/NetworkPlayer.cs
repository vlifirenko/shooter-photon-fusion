using Fusion;
using ShooterPhotonFusion.Camera;
using TMPro;
using UnityEngine;

namespace ShooterPhotonFusion.Network
{
    public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
    {
        private const string PLAYER_NICKNAME = "USER_001";

        public Transform playerModel;
        public TMP_Text playerNicknameText;
        public LocalCameraHandler localCameraHandler;
        public GameObject localUI;

        public static NetworkPlayer Local { get; set; }

        [Networked(OnChanged = nameof(OnNicknameChanged))]
        public NetworkString<_16> NickName { get; set; }

        [Networked] public int Token { get; set; }

        private bool _isPublicJoinMessageSent;
        private NetworkInGameMessages _networkInGameMessages;

        private void Awake()
        {
            _networkInGameMessages = GetComponent<NetworkInGameMessages>();
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
                Utils.Utils.SetRendererLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

                UnityEngine.Camera.main.gameObject.SetActive(false);

                RPC_SetNickName(PlayerPrefs.GetString("PlayerNickname", PLAYER_NICKNAME));

                Debug.Log("Spawned local player");
            }
            else
            {
                var localCamera = GetComponentInChildren<UnityEngine.Camera>();
                localCamera.enabled = false;

                var audioListener = GetComponentInChildren<AudioListener>();
                audioListener.enabled = false;

                localUI.SetActive(false);

                Debug.Log("Spawned remote player");
            }

            Runner.SetPlayerObject(Object.InputAuthority, Object);

            transform.name = $"P_{Object.Id}";
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (Object.HasStateAuthority)
            {
                if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetworkObject))
                {
                    if (playerLeftNetworkObject == Object)
                    {
                        Local.GetComponent<NetworkInGameMessages>()
                            .SendInGameRPCMessage(playerLeftNetworkObject.GetComponent<NetworkPlayer>().NickName.ToString(),
                                "left");
                    }
                }
            }

            if (player == Object.InputAuthority)
                Runner.Despawn(Object);
        }

        private static void OnNicknameChanged(Changed<NetworkPlayer> changed)
        {
            Debug.Log($"{Time.time} OnNicknameChanged value {changed.Behaviour.NickName}");

            changed.Behaviour.OnNicknameChanged();
        }

        private void OnNicknameChanged()
        {
            Debug.Log($"Nickname changed for player to {NickName} for player {gameObject.name}");

            playerNicknameText.text = NickName.ToString();
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetNickName(string nickName, RpcInfo info = default)
        {
            Debug.Log($"[RPC] SetNickName {nickName}");
            NickName = nickName;

            if (!_isPublicJoinMessageSent)
            {
                _networkInGameMessages.SendInGameRPCMessage(nickName, "joined");
                _isPublicJoinMessageSent = true;
            }
        }
    }
}