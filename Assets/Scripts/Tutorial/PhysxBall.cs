using Fusion;
using UnityEngine;

namespace ShooterPhotonFusion.Tutorial
{
    public class PhysxBall : NetworkBehaviour
    {
        [Networked] public TickTimer Life { get; set; }

        public void Init(Vector3 forward)
        {
            Life = TickTimer.CreateFromSeconds(Runner, 5f);
            GetComponent<Rigidbody>().velocity = forward;
        }

        public override void FixedUpdateNetwork()
        {
            if (Life.Expired(Runner))
                Runner.Despawn(Object);
        }
    }
}