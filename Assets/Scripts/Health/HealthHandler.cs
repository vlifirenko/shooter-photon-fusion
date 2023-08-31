using Fusion;
using UnityEngine;

namespace ShooterPhotonFusion.Health
{
    public class HealthHandler : NetworkBehaviour
    {
        private const byte StartingHealth = 5;
        
        [Networked(OnChanged = nameof(OnHpChanged))]
        private byte Health { get; set; }
        
        [Networked(OnChanged = nameof(OnStateChanged))]
        public bool IsDead { get; set; }

        private bool _isInitialized;

        private void Start()
        {
            Health = StartingHealth;
            IsDead = false;
        }

        // only called on server
        public void OnTakeDamage()
        {
            if (IsDead)
                return;

            Health -= 1;
            
            Debug.Log($"{Time.time} {transform.name} took damage {Health} left");

            if (Health <= 0)
            {
                Debug.Log($"{Time.time} {transform.name} died");

                IsDead = true;
            }
        }

        private static void OnHpChanged(Changed<HealthHandler> changed)
        {
            Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.Health}");
        }

        private static void OnStateChanged(Changed<HealthHandler> changed)
        {
            Debug.Log($"{Time.time} OnStateChanged value {changed.Behaviour.IsDead}");
        }
    }
}