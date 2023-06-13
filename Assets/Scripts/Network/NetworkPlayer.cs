using Fusion;
using UnityEngine;

namespace ShooterPhotonFusion.Network
{
    public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
    {
        [SerializeField] private Transform playerModel;
        
        public static NetworkPlayer Local { get; set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
                Utils.Utils.SetRendererLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));
                
                UnityEngine.Camera.main.gameObject.SetActive(false);
                
                Debug.Log("Spawned local player");
            }
            else
            {
                var localCamera = GetComponentInChildren<UnityEngine.Camera>();
                localCamera.enabled = false;

                var audioListener = GetComponentInChildren<AudioListener>();
                audioListener.enabled = false;
                
                Debug.Log("Spawned remote player");
            }

            transform.name = $"P_{Object.Id}";
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (player == Object.InputAuthority)
                Runner.Despawn(Object);
        }
    }
}