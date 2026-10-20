using UnityEngine;
using SiagaBencana.Core;

namespace SiagaBencana.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class CoverZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("[CoverZone] Masuk ke bawah meja kokoh! Tekan C untuk berlindung.");
            }
        }
    }
}
