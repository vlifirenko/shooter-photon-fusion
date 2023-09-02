using Fusion;
using UnityEngine;

namespace ShooterPhotonFusion.Tutorial
{
    public struct NetworkInputData : INetworkInput
    {
        public const byte MOUSEBUTTON1 = 0x01;

        public byte Buttons;
        public Vector3 Direction;
    }
}