using Fusion;
using UnityEngine;

namespace ShooterPhotonFusion.Tutorial
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Ball prefabBall;

        private NetworkCharacterControllerPrototype _cc;
        private Vector3 _forward;

        [Networked] private TickTimer delay { get; set; }

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterControllerPrototype>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.Direction.Normalize();
                _cc.Move(5 * data.Direction * Runner.DeltaTime);

                if (data.Direction.sqrMagnitude > 0)
                    _forward = data.Direction;

                if (delay.ExpiredOrNotRunning(Runner))
                {
                    if ((data.Buttons & NetworkInputData.MOUSEBUTTON1) != 0)
                    {
                        delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                        Runner.Spawn(prefabBall, transform.position + _forward, Quaternion.LookRotation(_forward),
                            Object.InputAuthority,
                            (runner, o) => o.GetComponent<Ball>().Init());
                    }
                }
            }
        }
    }
}