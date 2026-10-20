using UnityEngine;
using SiagaBencana.Core;

namespace SiagaBencana.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class SafeZone : MonoBehaviour
    {
        private bool _completed;

        private void OnTriggerEnter(Collider other)
        {
            if (_completed) return;
            if (GameManager.Instance.CurrentState != GameState.PostDisaster) return;

            var player = other.GetComponent<PlayerController>();
            if (player == null) return;

            _completed = true;
            GameEvents.RaisePlayerReachedSafeZone();
            GameEvents.RaiseLevelCompleted(true);
            GameManager.Instance.ShowDebrief();

            Debug.Log("[SafeZone] Berhasil sampai di titik kumpul aman!");
        }
    }
}
