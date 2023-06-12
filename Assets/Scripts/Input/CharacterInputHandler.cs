using System;
using ShooterPhotonFusion.Movement;
using UnityEngine;

namespace ShooterPhotonFusion.Input
{
    public class CharacterInputHandler : MonoBehaviour
    {
        private CharacterMovementHandler _characterMovementHandler;
        private Vector2 _moveInputVector = Vector2.zero;
        private Vector2 _viewInputVector = Vector2.zero;
        private bool _isJumpPressed;

        private void Awake()
        {
            _characterMovementHandler = GetComponent<CharacterMovementHandler>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            _viewInputVector.x = UnityEngine.Input.GetAxis("Mouse X");
            _viewInputVector.y = -UnityEngine.Input.GetAxis("Mouse Y");

            _characterMovementHandler.SetViewInputVector(_viewInputVector);
            
            _moveInputVector.x = UnityEngine.Input.GetAxis("Horizontal");
            _moveInputVector.y = UnityEngine.Input.GetAxis("Vertical");

            //_isJumpPressed = UnityEngine.Input.GetKeyDown("Jump");
        }

        public NetworkInputData GetNetworkInput()
        {
            var networkInputData = new NetworkInputData
            {
                MovementInput = _moveInputVector,
                RotationInput = _viewInputVector.x,
                IsJumpPressed = _isJumpPressed
            };
            return networkInputData;
        }
    }
}