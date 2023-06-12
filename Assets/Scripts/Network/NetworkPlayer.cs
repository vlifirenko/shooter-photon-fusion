using System;
using Fusion;
using UnityEngine;

namespace ShooterPhotonFusion.Network
{
    public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
    {
        public static NetworkPlayer Local { get; set; }

        private void Start()
        {
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
                Debug.Log("Spawned local player");
            }
            else
                Debug.Log("Spawned remote player");
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (player == Object.InputAuthority)
                Runner.Despawn(Object);
        }
    }
}