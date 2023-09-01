using System.Collections;
using TMPro;
using UnityEngine;

namespace ShooterPhotonFusion.Ui
{
    public class InGameMessageUiHandler : MonoBehaviour
    {
        public TMP_Text[] texts;

        private Queue _messageQueue = new Queue();

        public void OnGameMessageReceived(string message)
        {
            Debug.Log($"InGameMessageUiHandler {message}");

            _messageQueue.Enqueue(message);

            if (_messageQueue.Count > 3)
                _messageQueue.Dequeue();

            var queueIndex = 0;
            foreach (string messageInQueue in _messageQueue)
            {
                texts[queueIndex].text = messageInQueue;
                queueIndex++;
            }
        }
    }
}