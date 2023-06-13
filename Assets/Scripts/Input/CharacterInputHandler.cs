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
        private CharacterMovementHandler _characterMovementHandler;
        private bool _isJumpPressed;
        private bool _isFirePressed;

        private void Awake()
        {
            _localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
            _characterMovementHandler = GetComponent<CharacterMovementHandler>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (!_characterMovementHandler.Object.HasInputAuthority)
                return;

            _viewInputVector.x = UnityEngine.Input.GetAxis("Mouse X");
            _viewInputVector.y = -UnityEngine.Input.GetAxis("Mouse Y");
            
            _moveInputVector.x = UnityEngine.Input.GetAxis("Horizontal");
            _moveInputVector.y = UnityEngine.Input.GetAxis("Vertical");

            if (UnityEngine.Input.GetButtonDown("Jump"))
                _isJumpPressed = true;
            
            if (UnityEngine.Input.GetButtonDown("Fire1"))
                _isFirePressed = true;
            
            _localCameraHandler.SetViewInputVector(_viewInputVector);
        }

        public NetworkInputData GetNetworkInput()
        {
            var networkInputData = new NetworkInputData
            {
                MovementInput = _moveInputVector,
                AimForwardVector = _localCameraHandler.transform.forward,
                IsJumpPressed = _isJumpPressed,
                IsFirePressed =  _isFirePressed,
            };

            _isJumpPressed = false;
            _isFirePressed = false;
            
            return networkInputData;
        }
    }
}