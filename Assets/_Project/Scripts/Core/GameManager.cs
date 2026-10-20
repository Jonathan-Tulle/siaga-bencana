using UnityEngine;
using UnityEngine.SceneManagement;

namespace SiagaBencana.Core
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GameState _currentState = GameState.Boot;

        public GameState CurrentState => _currentState;
        public int CurrentLevelIndex { get; private set; } = 1;
        public DisasterType CurrentDisasterType { get; private set; } = DisasterType.Earthquake;

        public void ChangeState(GameState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;
            Debug.Log($"[GameManager] State -> {newState}");
            GameEvents.RaiseGameStateChanged(newState);
            HandleStateTransition(newState);
        }

        public void LoadLevel(int levelIndex)
        {
            CurrentLevelIndex = levelIndex;
            CurrentDisasterType = DisasterType.Earthquake;
            ChangeState(GameState.Preparation);
        }

        public void StartDisasterPhase() => ChangeState(GameState.Disaster);
        public void StartPostDisasterPhase() => ChangeState(GameState.PostDisaster);
        public void ShowDebrief() => ChangeState(GameState.Debrief);

        public void PauseGame()
        {
            if (_currentState == GameState.Paused) return;
            Time.timeScale = 0f;
            ChangeState(GameState.Paused);
        }

        public void ResumeGame(GameState returnState)
        {
            Time.timeScale = 1f;
            ChangeState(returnState);
        }

        private void HandleStateTransition(GameState state)
        {
            switch (state)
            {
                case GameState.Tutorial:
                    Time.timeScale = 0f; // Pause saat tutorial
                    break;
                case GameState.Preparation:
                case GameState.Disaster:
                case GameState.PostDisaster:
                    Time.timeScale = 1f;
                    if (state == GameState.Disaster)
                        GameEvents.RaiseDisasterStarted(CurrentDisasterType);
                    if (state == GameState.PostDisaster)
                        GameEvents.RaiseDisasterEnded();
                    break;
                case GameState.Debrief:
                    Time.timeScale = 0f;
                    break;
            }
        }
    }
}
