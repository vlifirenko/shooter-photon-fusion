using Fusion;

namespace ShooterPhotonFusion.Movement
{
    public class CharacterMovementHandler : NetworkBehaviour
    {
        private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;

        private void Awake()
        {
            _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData networkInputData))
            {
                var moveDirection = transform.forward * networkInputData.MovementInput.y + 
                                    transform.right * networkInputData.MovementInput.x;
                moveDirection.Normalize();
                
                _networkCharacterControllerPrototypeCustom.Move(moveDirection);
            }
        }
    }
}