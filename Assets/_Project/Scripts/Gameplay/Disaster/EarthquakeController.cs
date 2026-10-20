using UnityEngine;
using SiagaBencana.Core;

namespace SiagaBencana.Gameplay
{
    public class EarthquakeController : MonoBehaviour
    {
        [SerializeField] private float _earthquakeDuration = 12f;

        private EarthquakePhase _currentPhase = EarthquakePhase.Idle;
        private float _timer;
        private bool _isActive;
        private bool _decisionTriggered;

        public EarthquakePhase CurrentPhase => _currentPhase;

        private void OnEnable()
        {
            GameEvents.OnDisasterStarted += HandleDisasterStarted;
        }

        private void OnDisable()
        {
            GameEvents.OnDisasterStarted -= HandleDisasterStarted;
        }

        private void Update()
        {
            if (!_isActive) return;

            _timer -= Time.deltaTime;

            if (!_decisionTriggered && (_earthquakeDuration - _timer) >= 1f)
            {
                _decisionTriggered = true;
                GameEvents.RaiseDecisionTriggered();
            }

            if (_timer <= 0f)
            {
                EndEarthquake();
            }
        }

        private void HandleDisasterStarted(DisasterType type)
        {
            _isActive = true;
            _decisionTriggered = false;
            _currentPhase = EarthquakePhase.Shaking;
            _timer = _earthquakeDuration;
            GameEvents.RaiseEarthquakePhaseChanged(EarthquakePhase.Shaking);
        }

        private void EndEarthquake()
        {
            _isActive = false;
            _currentPhase = EarthquakePhase.Ended;
            GameEvents.RaiseEarthquakePhaseChanged(EarthquakePhase.Ended);
            GameManager.Instance.StartPostDisasterPhase();
        }
    }
}
