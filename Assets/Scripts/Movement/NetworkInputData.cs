using Fusion;
using UnityEngine;

namespace ShooterPhotonFusion.Movement
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 MovementInput;
        public float RotationInput;
        public NetworkBool IsJumpPressed;
    }
}