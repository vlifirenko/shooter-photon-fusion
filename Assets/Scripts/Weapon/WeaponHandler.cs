using System.Collections;
using Fusion;
using ShooterPhotonFusion.Movement;
using UnityEngine;

namespace ShooterPhotonFusion.Weapon
{
    public class WeaponHandler : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(OnFireChanged))]
        public bool IsFiring { get; set; }

        private float _lastTimeFired;

        public override void FixedUpdateNetwork()
        {
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
            Debug.Log($"{Time.time} OnFireChanged value {changed.Behaviour.IsFiring}");

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