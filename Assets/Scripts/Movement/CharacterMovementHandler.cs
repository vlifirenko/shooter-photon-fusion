using Fusion;
using UnityEngine;

namespace ShooterPhotonFusion.Movement
{
    public class CharacterMovementHandler : NetworkBehaviour
    {
        private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
        private UnityEngine.Camera _localCamera;

        private void Awake()
        {
            _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
            _localCamera = GetComponentInChildren<UnityEngine.Camera>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData networkInputData))
            {
                transform.forward = networkInputData.AimForwardVector;
                var rotation = transform.rotation;
                rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
                transform.rotation = rotation;
                
                var moveDirection = transform.forward * networkInputData.MovementInput.y +
                                    transform.right * networkInputData.MovementInput.x;
                moveDirection.Normalize();
                _networkCharacterControllerPrototypeCustom.Move(moveDirection);

                if (networkInputData.IsJumpPressed)
                    _networkCharacterControllerPrototypeCustom.Jump();
                
                CheckFallRespawn();
            }
        }

        private void CheckFallRespawn()
        {
            if (transform.position.y < -12)
                transform.position = Utils.Utils.GetRandomSpawnPoint();
        }
    }
}