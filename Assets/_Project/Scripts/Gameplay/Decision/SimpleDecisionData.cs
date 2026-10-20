using UnityEngine;

namespace SiagaBencana.Gameplay
{
    [System.Serializable]
    public class SimpleChoice
    {
        public string text;
        [TextArea(2, 3)] public string feedback;
        public bool isCorrect;
    }

    [CreateAssetMenu(fileName = "NewSimpleDecision", menuName = "Siaga Bencana/Simple Decision")]
    public class SimpleDecisionData : ScriptableObject
    {
        public string decisionId;
        [TextArea(2, 3)] public string question;
        public SimpleChoice[] choices;
    }
}
