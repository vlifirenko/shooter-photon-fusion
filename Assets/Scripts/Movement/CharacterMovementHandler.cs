﻿using Fusion;
using ShooterPhotonFusion.Health;
using UnityEngine;

namespace ShooterPhotonFusion.Movement
{
    public class CharacterMovementHandler : NetworkBehaviour
    {
        private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
        private HealthHandler _healthHandler;
        
        private bool _isRespawnRequsted = false;

        private void Awake()
        {
            _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
            _healthHandler = GetComponent<HealthHandler>();
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                if (_isRespawnRequsted)
                {
                    Respawn();
                    return;
                }
                
                if (_healthHandler.IsDead)
                    return;
            }
                

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
            {
                if (Object.HasStateAuthority)
                {
                    Respawn();
                }
            }
        }

        public void RequestRespawn() => _isRespawnRequsted = true;

        private void Respawn()
        {
            _networkCharacterControllerPrototypeCustom.TeleportToPosition(Utils.Utils.GetRandomSpawnPoint());

            _healthHandler.OnRespawned();
            
            _isRespawnRequsted = false;
        }

        public void SetCharacterControllerEnabled(bool isEnabled) => _networkCharacterControllerPrototypeCustom.Controller.enabled = isEnabled;
    }
}