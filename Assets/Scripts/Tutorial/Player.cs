using System;
using Fusion;
using TMPro;
using UnityEngine;

namespace ShooterPhotonFusion.Tutorial
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Ball prefabBall;
        [SerializeField] private PhysxBall prefabPhysxBall;

        [Networked(OnChanged = nameof(OnBallSpawned))]
        public NetworkBool Spawned { get; set; }

        private NetworkCharacterControllerPrototype _cc;
        private TMP_Text _messages;

        private Vector3 _forward;

        [Networked] private TickTimer delay { get; set; }

        private void Awake() => _cc = GetComponent<NetworkCharacterControllerPrototype>();

        private void Update()
        {
            if (Object.HasInputAuthority && UnityEngine.Input.GetKeyDown(KeyCode.R))
                RPC_SendMessage("Hey Mate!");
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
                        Spawned = !Spawned;
                    }
                    else if ((data.Buttons & NetworkInputData.MOUSEBUTTON2) != 0)
                    {
                        delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                        Runner.Spawn(prefabPhysxBall,
                            transform.position + _forward,
                            Quaternion.LookRotation(_forward),
                            Object.InputAuthority,
                            (runner, o) => o.GetComponent<PhysxBall>().Init(10 * _forward));
                        Spawned = !Spawned;
                    }
                }
            }
        }

        public override void Render()
        {
            var material = GetComponentInChildren<Renderer>().material;
            material.color = Color.Lerp(material.color, Color.blue, Time.deltaTime);
        }

        public static void OnBallSpawned(Changed<Player> changed)
            => changed.Behaviour.GetComponentInChildren<Renderer>().material.color = Color.white;

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_SendMessage(string message, RpcInfo info = default)
        {
            if (message == null)
                _messages = FindObjectOfType<TMP_Text>();
            
            if (info.IsInvokeLocal)
                message = $"You said: {message}\n";
            else
                message = $"Some other player said: {message}\n";

            _messages.text += message;
        }
    }
}