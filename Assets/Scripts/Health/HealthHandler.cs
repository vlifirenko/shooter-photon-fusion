using System;
using System.Collections;
using Fusion;
using ShooterPhotonFusion.Movement;
using ShooterPhotonFusion.Network;
using UnityEngine;
using UnityEngine.UI;
using NetworkPlayer = ShooterPhotonFusion.Network.NetworkPlayer;

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

        public Color uiOnHitColor;
        public Image uiOnHitImage;

        public MeshRenderer bodyMeshRenderer;
        private Color _defaultMeshColorBody;

        public GameObject playerModel;
        public GameObject deathGameObjectPrefab;

        private HitboxRoot _hitboxRoot;
        private CharacterMovementHandler _characterMovementHandler;
        private NetworkPlayer _networkPlayer;
        private NetworkInGameMessages _networkInGameMessages;

        private void Awake()
        {
            _characterMovementHandler = GetComponent<CharacterMovementHandler>();
            _hitboxRoot = GetComponent<HitboxRoot>();
            _networkPlayer = GetComponent<NetworkPlayer>();
            _networkInGameMessages = GetComponent<NetworkInGameMessages>();
        }

        private void Start()
        {
            Health = StartingHealth;
            IsDead = false;

            _defaultMeshColorBody = bodyMeshRenderer.material.color;

            _isInitialized = true;
        }

        private IEnumerator OnHitCo()
        {
            bodyMeshRenderer.material.color = Color.white;

            if (Object.HasInputAuthority)
                uiOnHitImage.color = uiOnHitColor;

            yield return new WaitForSeconds(0.2f);

            bodyMeshRenderer.material.color = _defaultMeshColorBody;

            if (Object.HasInputAuthority && !IsDead)
                uiOnHitImage.color = new Color(0, 0, 0, 0);
        }

        private IEnumerator ServerReviveCo()
        {
            yield return new WaitForSeconds(2f);

            _characterMovementHandler.RequestRespawn();
        }

        // only called on server
        public void OnTakeDamage(string damageCausedByPlayerNickname)
        {
            if (IsDead)
                return;

            Health -= 1;

            Debug.Log($"{Time.time} {transform.name} took damage {Health} left");

            if (Health <= 0)
            {
                _networkInGameMessages.SendInGameRPCMessage(damageCausedByPlayerNickname,
                    $"Killed <b>{_networkPlayer.NickName.ToString()}</b>");

                Debug.Log($"{Time.time} {transform.name} died");

                StartCoroutine(ServerReviveCo());

                IsDead = true;
            }
        }

        private static void OnHpChanged(Changed<HealthHandler> changed)
        {
            Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.Health}");

            var newHealth = changed.Behaviour.Health;
            changed.LoadOld();
            var oldHealth = changed.Behaviour.Health;

            if (newHealth < oldHealth)
                changed.Behaviour.OnHealthReduced();
        }

        private void OnHealthReduced()
        {
            if (!_isInitialized)
                return;

            StartCoroutine(OnHitCo());
        }

        private static void OnStateChanged(Changed<HealthHandler> changed)
        {
            Debug.Log($"{Time.time} OnStateChanged value {changed.Behaviour.IsDead}");

            var isDeadCurrent = changed.Behaviour.IsDead;
            changed.LoadOld();
            var isDeadOld = changed.Behaviour.IsDead;

            if (isDeadCurrent)
                changed.Behaviour.OnDeath();
            else if (isDeadOld)
                changed.Behaviour.OnRevive();
        }

        private void OnDeath()
        {
            Debug.Log($"{Time.time} OnDeath");

            playerModel.gameObject.SetActive(false);
            _hitboxRoot.HitboxRootActive = false;
            _characterMovementHandler.SetCharacterControllerEnabled(false);

            // particles
            //Instantiate(deathGameObjectPrefab, transform.position, Quaternion.identity);
        }

        private void OnRevive()
        {
            Debug.Log($"{Time.time} OnRevive");

            if (Object.HasInputAuthority)
                uiOnHitImage.color = new Color(0, 0, 0, 0);

            playerModel.gameObject.SetActive(true);
            _hitboxRoot.HitboxRootActive = true;
            _characterMovementHandler.SetCharacterControllerEnabled(true);
        }

        public void OnRespawned()
        {
            Health = StartingHealth;
            IsDead = false;
        }
    }
}