using System;
using ShooterPhotonFusion.Camera;
using ShooterPhotonFusion.Movement;
using UnityEngine;

namespace ShooterPhotonFusion.Input
{
    public class CharacterInputHandler : MonoBehaviour
    {
        private LocalCameraHandler _localCameraHandler;
        private Vector2 _moveInputVector = Vector2.zero;
        private Vector2 _viewInputVector = Vector2.zero;
        private bool _isJumpPressed;

        private void Awake()
        {
            _localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
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
            
            _moveInputVector.x = UnityEngine.Input.GetAxis("Horizontal");
            _moveInputVector.y = UnityEngine.Input.GetAxis("Vertical");

            if (UnityEngine.Input.GetButtonDown("Jump"))
                _isJumpPressed = true;
            
            _localCameraHandler.SetViewInputVector(_viewInputVector);
        }

        public NetworkInputData GetNetworkInput()
        {
            var networkInputData = new NetworkInputData
            {
                MovementInput = _moveInputVector,
                AimForwardVector = _localCameraHandler.transform.forward,
                IsJumpPressed = _isJumpPressed
            };

            _isJumpPressed = false;
            
            return networkInputData;
        }
    }
}