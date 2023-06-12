using System;
using ShooterPhotonFusion.Movement;
using UnityEngine;

namespace ShooterPhotonFusion.Input
{
    public class CharacterInputHandler : MonoBehaviour
    {
        private Vector2 _moveInputVector = Vector2.zero;

        private void Update()
        {
            _moveInputVector.x = UnityEngine.Input.GetAxis("Horizontal");
            _moveInputVector.y = UnityEngine.Input.GetAxis("Vertical");
        }

        public NetworkInputData GetNetworkInput()
        {
            var networkInputData = new NetworkInputData
            {
                MovementInput = _moveInputVector
            };
            return networkInputData;
        }
    }
}