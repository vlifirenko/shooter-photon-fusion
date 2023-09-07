using System;
using ShooterPhotonFusion.Network;
using UnityEngine;

namespace ShooterPhotonFusion
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private byte[] _connectionToken;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (_connectionToken == null)
                _connectionToken = ConnectionTokenUtils.NewToken();
        }

        public void SetConnectionToken(byte[] connectionToken) => _connectionToken = connectionToken;

        public byte[] GetConnectionToken() => _connectionToken;
    }
}