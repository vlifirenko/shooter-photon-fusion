using System;
using System.Collections;
using Fusion;
using ShooterPhotonFusion.Health;
using ShooterPhotonFusion.Movement;
using UnityEngine;
using NetworkPlayer = ShooterPhotonFusion.Network.NetworkPlayer;

namespace ShooterPhotonFusion.Weapon
{
    public class WeaponHandler : NetworkBehaviour
    {
        [SerializeField] private Transform aimPoint;
        [SerializeField] private LayerMask collisionLayers;

        [Networked(OnChanged = nameof(OnFireChanged))]
        public bool IsFiring { get; set; }

        private HealthHandler _healthHandler;
        private NetworkPlayer _networkPlayer;
        private float _lastTimeFired;

        private void Awake()
        {
            _healthHandler = GetComponent<HealthHandler>();
            _networkPlayer = GetComponent<NetworkPlayer>();
        }

        public override void FixedUpdateNetwork()
        {
            if (_healthHandler.IsDead)
                return;
            
            if (GetInput(out NetworkInputData networkInputData))
            {
                if (networkInputData.IsFirePressed)
                    Fire(networkInputData.AimForwardVector);
            }
        }

        private void Fire(Vector3 aimForwardVector)
        {
            if (Time.time - _lastTimeFired < 0.1f)
                return;

            StartCoroutine(FireEffectCo());

            Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, 100, Object.InputAuthority, out var hitInfo,
                collisionLayers, HitOptions.IgnoreInputAuthority);

            var hitDistance = 100f;
            var isHitOtherPlayer = false;

            if (hitInfo.Distance > 0)
                hitDistance = hitInfo.Distance;

            if (hitInfo.Hitbox != null)
            {
                Debug.Log($"{Time.time} {transform.name} hit hitbox {hitInfo.Hitbox.transform.root.name}");

                if (Object.HasStateAuthority)
                    hitInfo.Hitbox.transform.root.GetComponent<HealthHandler>().OnTakeDamage(_networkPlayer.NickName.ToString());
                
                isHitOtherPlayer = true;
            }
            else if (hitInfo.Collider != null)
            {
                Debug.Log($"{Time.time} {transform.name} hit PhysX {hitInfo.Collider.transform.name}");
            }

            // debug
            if (isHitOtherPlayer)
                Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.red, 1);
            else
                Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.green, 1);

            _lastTimeFired = Time.time;
        }

        private IEnumerator FireEffectCo()
        {
            IsFiring = true;
            // play particles

            yield return new WaitForSeconds(0.09f);

            IsFiring = false;
        }

        private static void OnFireChanged(Changed<WeaponHandler> changed)
        {
            //Debug.Log($"{Time.time} OnFireChanged value {changed.Behaviour.IsFiring}");

            var isFiringCurrent = changed.Behaviour.IsFiring;
            changed.LoadOld();

            var isFiringOld = changed.Behaviour.IsFiring;

            if (isFiringCurrent && !isFiringOld)
                changed.Behaviour.OnFireRemote();
        }

        private void OnFireRemote()
        {
            if (!Object.HasInputAuthority)
            {
                // play particles
            }
        }
    }
}