using UnityEngine;
using SiagaBencana.Core;

namespace SiagaBencana.Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private float _preparationTime = 15f;

        private float _currentTimer;
        private bool _timerActive;

        public float CurrentTimer => _currentTimer;

        private void OnEnable()
        {
            GameEvents.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void Update()
        {
            if (!_timerActive) return;

            _currentTimer -= Time.deltaTime;
            if (_currentTimer <= 0f)
            {
                _currentTimer = 0f;
                _timerActive = false;
                HandlePreparationExpired();
            }
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.Preparation)
            {
                _currentTimer = _preparationTime;
                _timerActive = true;
            }
            else
            {
                _timerActive = false;
            }
        }

        private void HandlePreparationExpired()
        {
            // Transisi otomatis dari Persiapan -> Gempa
            GameManager.Instance.StartDisasterPhase();
        }
    }
}
