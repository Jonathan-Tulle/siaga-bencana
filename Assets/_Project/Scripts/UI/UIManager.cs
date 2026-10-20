using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SiagaBencana.Core;
using SiagaBencana.Gameplay;

namespace SiagaBencana.UI
{
    [System.Serializable]
    public struct TutorialStep
    {
        public string title;
        [TextArea(2, 4)] public string description;
        public string controlHint;
    }

    [DefaultExecutionOrder(10)]
    public class UIManager : MonoBehaviour
    {
        [Header("Tutorial UI")]
        [SerializeField] private GameObject _tutorialPanel;
        [SerializeField] private TextMeshProUGUI _tutorialTitle;
        [SerializeField] private TextMeshProUGUI _tutorialDesc;
        [SerializeField] private TextMeshProUGUI _tutorialControl;
        [SerializeField] private TextMeshProUGUI _tutorialStepCounter;
        [SerializeField] private Button _tutorialNextBtn;
        [SerializeField] private TextMeshProUGUI _tutorialNextBtnText;

        [SerializeField] private TutorialStep[] _tutorialSteps = new TutorialStep[]
        {
            new TutorialStep {
                title = "Selamat Datang di Siaga Bencana!",
                description = "Pelajari langkah mitigasi gempa bumi sederhana sesuai pedoman BNPB.",
                controlHint = ""
            },
            new TutorialStep {
                title = "Gerakan Dasar",
                description = "Gunakan tombol arah untuk bergerak dan berlari di dalam ruangan.",
                controlHint = "W A S D = Jalan\nSHIFT (tahan) = Lari"
            },
            new TutorialStep {
                title = "Berlindung: Drop, Cover, Hold On",
                description = "Saat gempa terjadi, segera cari meja kokoh lalu tekan tombol merunduk untuk melindungi kepala!",
                controlHint = "C = Merunduk / Berlindung"
            },
            new TutorialStep {
                title = "Evakuasi ke Titik Kumpul",
                description = "Setelah guncangan gempa reda, segera lari keluar menuju area aman di halaman!",
                controlHint = "Lari ke area hijau di luar rumah"
            }
        };

        private int _tutorialIndex;

        [Header("HUD")]
        [SerializeField] private GameObject _hudPanel;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private TextMeshProUGUI _phaseText;
        [SerializeField] private GameObject _warningBanner;
        [SerializeField] private TextMeshProUGUI _warningText;

        [Header("Decision UI")]
        [SerializeField] private GameObject _decisionPanel;
        [SerializeField] private TextMeshProUGUI _decisionQuestionText;
        [SerializeField] private Button _choiceBtn1;
        [SerializeField] private TextMeshProUGUI _choiceTxt1;
        [SerializeField] private Button _choiceBtn2;
        [SerializeField] private TextMeshProUGUI _choiceTxt2;

        [SerializeField] private GameObject _feedbackPanel;
        [SerializeField] private TextMeshProUGUI _feedbackTitle;
        [SerializeField] private TextMeshProUGUI _feedbackDesc;
        [SerializeField] private Button _feedbackContinueBtn;

        [Header("Debrief UI")]
        [SerializeField] private GameObject _debriefPanel;
        [SerializeField] private TextMeshProUGUI _debriefTitle;
        [SerializeField] private TextMeshProUGUI _debriefStatus;
        [SerializeField] private Button _debriefRestartBtn;

        private LevelManager _levelManager;

        private void Awake()
        {
            GameEvents.OnGameStateChanged += OnGameStateChanged;
            GameEvents.OnEarthquakePhaseChanged += OnEarthquakePhase;
            GameEvents.OnDecisionTriggered += ShowDecisionDialog;
            GameEvents.OnLevelCompleted += OnLevelCompleted;

            SetPanel(_tutorialPanel, false);
            SetPanel(_hudPanel, false);
            SetPanel(_decisionPanel, false);
            SetPanel(_feedbackPanel, false);
            SetPanel(_debriefPanel, false);
            SetPanel(_warningBanner, false);

            if (_tutorialNextBtn != null)
                _tutorialNextBtn.onClick.AddListener(OnTutorialNext);

            if (_choiceBtn1 != null)
                _choiceBtn1.onClick.AddListener(() => OnChoiceSelected(true));

            if (_choiceBtn2 != null)
                _choiceBtn2.onClick.AddListener(() => OnChoiceSelected(false));

            if (_feedbackContinueBtn != null)
                _feedbackContinueBtn.onClick.AddListener(OnFeedbackContinue);

            if (_debriefRestartBtn != null)
                _debriefRestartBtn.onClick.AddListener(OnRestartLevel);
        }

        private void OnDestroy()
        {
            GameEvents.OnGameStateChanged -= OnGameStateChanged;
            GameEvents.OnEarthquakePhaseChanged -= OnEarthquakePhase;
            GameEvents.OnDecisionTriggered -= ShowDecisionDialog;
            GameEvents.OnLevelCompleted -= OnLevelCompleted;
        }

        private void Start()
        {
            _levelManager = FindAnyObjectByType<LevelManager>();
            ShowTutorial();
        }

        private void Update()
        {
            UpdateTimer();

            if (_tutorialPanel != null && _tutorialPanel.activeSelf)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb != null && (kb.spaceKey.wasPressedThisFrame || kb.enterKey.wasPressedThisFrame))
                {
                    OnTutorialNext();
                }
            }
        }

        public void ShowTutorial()
        {
            _tutorialIndex = 0;
            GameManager.Instance.ChangeState(GameState.Tutorial);
            SetPanel(_tutorialPanel, true);
            SetPanel(_hudPanel, false);
            DisplayTutorialStep();
        }

        private void DisplayTutorialStep()
        {
            if (_tutorialIndex >= _tutorialSteps.Length)
            {
                EndTutorial();
                return;
            }

            var step = _tutorialSteps[_tutorialIndex];
            if (_tutorialTitle != null) _tutorialTitle.text = step.title;
            if (_tutorialDesc != null) _tutorialDesc.text = step.description;
            if (_tutorialControl != null)
            {
                _tutorialControl.text = step.controlHint;
                _tutorialControl.gameObject.SetActive(!string.IsNullOrEmpty(step.controlHint));
            }
            if (_tutorialStepCounter != null)
                _tutorialStepCounter.text = $"{_tutorialIndex + 1} / {_tutorialSteps.Length}";

            bool isLast = _tutorialIndex == _tutorialSteps.Length - 1;
            if (_tutorialNextBtnText != null)
                _tutorialNextBtnText.text = isLast ? "MULAI BERMAIN!" : "Lanjut >>";
        }

        private void OnTutorialNext()
        {
            _tutorialIndex++;
            DisplayTutorialStep();
        }

        private void EndTutorial()
        {
            SetPanel(_tutorialPanel, false);
            SetPanel(_hudPanel, true);
            Time.timeScale = 1f;
            GameManager.Instance.LoadLevel(1);
        }

        private void UpdateTimer()
        {
            if (_levelManager == null || _timerText == null) return;
            float t = _levelManager.CurrentTimer;
            int m = Mathf.FloorToInt(t / 60f);
            int s = Mathf.FloorToInt(t % 60f);
            _timerText.text = $"{m:00}:{s:00}";
        }

        private void OnGameStateChanged(GameState state)
        {
            if (_phaseText == null) return;

            switch (state)
            {
                case GameState.Preparation:
                    _phaseText.text = "FASE PERSIAPAN (Eksplorasi Ruangan)";
                    SetPanel(_warningBanner, false);
                    break;
                case GameState.Disaster:
                    _phaseText.text = "GEMPA BUMI TERJADI!";
                    ShowWarning("GEMPA! Cari meja kokoh dan tekan C untuk berlindung!");
                    break;
                case GameState.PostDisaster:
                    _phaseText.text = "FASE EVAKUASI";
                    ShowWarning("Gempa reda! Segera lari ke titik kumpul di luar rumah!");
                    break;
                case GameState.Debrief:
                    _phaseText.text = "";
                    SetPanel(_warningBanner, false);
                    SetPanel(_hudPanel, false);
                    break;
            }
        }

        private void OnEarthquakePhase(EarthquakePhase phase)
        {
            if (phase == EarthquakePhase.Ended)
            {
                ShowWarning("Guncangan selesai! Keluar ke halaman!");
            }
        }

        private void ShowWarning(string msg)
        {
            SetPanel(_warningBanner, true);
            if (_warningText != null) _warningText.text = msg;
        }

        private void ShowDecisionDialog()
        {
            Time.timeScale = 0f;
            SetPanel(_decisionPanel, true);

            if (_decisionQuestionText != null)
                _decisionQuestionText.text = "Guncangan gempa bumi tiba-tiba terasa sangat kuat!\nApa tindakan paling tepat yang harus kamu lakukan?";

            if (_choiceTxt1 != null)
                _choiceTxt1.text = "A. Merunduk di bawah meja kokoh (Drop, Cover, Hold On)";

            if (_choiceTxt2 != null)
                _choiceTxt2.text = "B. Menerobos guncangan dan langsung berlari keluar";
        }

        private void OnChoiceSelected(bool isCorrect)
        {
            SetPanel(_decisionPanel, false);
            SetPanel(_feedbackPanel, true);

            if (isCorrect)
            {
                if (_feedbackTitle != null) _feedbackTitle.text = "<color=#2ECC71>TINDAKAN TEPAT! (BNPB)</color>";
                if (_feedbackDesc != null) _feedbackDesc.text = "Benar sekali! Saat gempa sedang berlangsung kencang, berlari keluar rumah sangat berisiko tertimpa genteng, kaca, atau benda jatuh. Berlindung di bawah meja kokoh melindungi organ vital tubuhmu.";
            }
            else
            {
                if (_feedbackTitle != null) _feedbackTitle.text = "<color=#E74C3C>KURANG TEPAT! (BNPB)</color>";
                if (_feedbackDesc != null) _feedbackDesc.text = "Hati-hati! Menerobos guncangan aktif berisiko tinggi membuatmu terpeleset atau tertimpa reruntuhan plafon. Pedoman BNPB menyarankan: Drop, Cover, and Hold On terlebih dahulu!";
            }
        }

        private void OnFeedbackContinue()
        {
            SetPanel(_feedbackPanel, false);
            Time.timeScale = 1f; // Lanjutkan simulasi gempa
        }

        private void OnLevelCompleted(bool success)
        {
            SetPanel(_debriefPanel, true);

            if (_debriefTitle != null)
                _debriefTitle.text = success ? "EVAKUASI BERHASIL!" : "SIMULASI BERAKHIR";

            if (_debriefStatus != null)
                _debriefStatus.text = "Hebat! Anda telah berhasil mempraktikkan langkah mitigasi bencana gempa bumi sesuai pedoman BNPB.";
        }

        private void OnRestartLevel()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        private void SetPanel(GameObject panel, bool active)
        {
            if (panel != null) panel.SetActive(active);
        }
    }
}
