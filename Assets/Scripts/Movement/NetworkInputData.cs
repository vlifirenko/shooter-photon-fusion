using Fusion;
using UnityEngine;

namespace ShooterPhotonFusion.Movement
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 MovementInput;
        public Vector3 AimForwardVector;
        public NetworkBool IsJumpPressed;
        public NetworkBool IsFirePressed;
    }
}