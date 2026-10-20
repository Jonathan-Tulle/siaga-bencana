namespace SiagaBencana.Core
{
    public enum GameState
    {
        Boot,
        Tutorial,
        Preparation,
        Disaster,
        PostDisaster,
        Debrief,
        Paused
    }

    public enum DisasterType
    {
        Earthquake
    }

    public enum EarthquakePhase
    {
        Idle,
        Shaking,
        Ended
    }

    public enum PlayerAction
    {
        Idle,
        Walking,
        Running,
        Covering
    }
}
