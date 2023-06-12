using Fusion;
using UnityEngine;

namespace ShooterPhotonFusion.Movement
{
    public class CharacterMovementHandler : NetworkBehaviour
    {
        private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
        private Camera _localCamera;
        private Vector2 _viewInput;
        private float _cameraRotationX;

        private void Awake()
        {
            _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
            _localCamera = GetComponentInChildren<Camera>();
        }

        private void Update()
        {
            _cameraRotationX += _viewInput.y * Time.deltaTime * _networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
            _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90, 90);

            _localCamera.transform.localRotation = Quaternion.Euler(_cameraRotationX, 0, 0);
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData networkInputData))
            {
                _networkCharacterControllerPrototypeCustom.Rotate(networkInputData.RotationInput);
                
                var moveDirection = transform.forward * networkInputData.MovementInput.y +
                                    transform.right * networkInputData.MovementInput.x;
                moveDirection.Normalize();
                _networkCharacterControllerPrototypeCustom.Move(moveDirection);

                if (networkInputData.IsJumpPressed)
                    _networkCharacterControllerPrototypeCustom.Jump();
            }
        }

        public void SetViewInputVector(Vector2 viewInput) => _viewInput = viewInput;
    }
}