using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO; // Add this at the top if it's not already there

public class TriviaManager : MonoBehaviour
{
    public static TriviaManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;

    [Header("Visual FX")]
    public GameObject correctFXPrefab;
    public GameObject wrongFXPrefab;
    public float fxSpawnYOffset = 0.5f;

    [Header("Reward Settings")]
    public List<GameObject> rewardPrefabs;
    public Transform rewardSpawnPoint;

    [Header("Reward SFX")]
    public AudioClip rewardSpawnClip;

    [Header("Audio & Haptics")]
    public AudioClip correctClip;
    public AudioClip wrongClip;
    [Range(0f, 1f)] public float hapticAmplitude = 0.5f;
    public float hapticDuration = 0.1f;
    private AudioSource audioSource;

    [Header("Trivia Data")]
    public List<TriviaQuestion> questions = new List<TriviaQuestion>();

    [System.Serializable]
    public class TriviaQuestion
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;
        public string rewardPrefabID;
    }

    private List<TriviaQuestion> pool;
    private int currentQuestionIndex = 0;

    public int CurrentScore = 0;

    private const int MaxQuestions = 20;
    private int questionsAnswered = 0;
    private int currentIndex = 0;

    public List<TriviaQuestion> allQuestions = new List<TriviaQuestion>();

    [System.Serializable]
    public class TriviaHistoryItem
    {
        public string questionText;
        public bool wasCorrect;
        public string rewardID;
        public TriviaHistoryItem(string q, bool c, string r)
        {
            questionText = q;
            wasCorrect = c;
            rewardID = r;
        }
    }

    [System.Serializable]
    public class MonumentProgress
    {
        public string monumentID;
        public int unlockedCount;
        public int totalPossible;
    }

    public List<TriviaHistoryItem> history = new List<TriviaHistoryItem>();
    public List<MonumentProgress> monumentProgress = new List<MonumentProgress>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        currentIndex = 0;
        questionsAnswered = 0;

        CurrentScore = PlayerPrefs.GetInt("TriviaScore", 0);
    }

    void Start()
    {
        // Ensure questions are loaded from JSON before using GetNextQuestion
        questions = LoadFromJSON();
        pool = new List<TriviaQuestion>(questions);
        Shuffle(pool);
        currentQuestionIndex = 0;
        questionsAnswered = 0;
        UpdateScoreText();
        InitializeMonumentProgress();
    }

    private void InitializeMonumentProgress()
    {
        monumentProgress.Clear();
        foreach (var q in questions)
        {
            if (!monumentProgress.Exists(m => m.monumentID == q.rewardPrefabID))
            {
                int total = questions.Count(x => x.rewardPrefabID == q.rewardPrefabID);
                monumentProgress.Add(new MonumentProgress() {
                    monumentID = q.rewardPrefabID,
                    unlockedCount = 0,
                    totalPossible = total
                });
            }
        }

        foreach (var mp in monumentProgress)
            mp.unlockedCount = PlayerPrefs.GetInt($"Unlocked_{mp.monumentID}", 0);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    public TriviaQuestion GetNextQuestion()
    {
        // Restore: just use pool and questionsAnswered
        if (pool == null || pool.Count == 0)
        {
            Debug.LogWarning("[TriviaManager] No questions in pool!");
            return null;
        }

        if (currentQuestionIndex < pool.Count && questionsAnswered < MaxQuestions)
        {
            var q = pool[currentQuestionIndex];
            currentQuestionIndex++;
            return q;
        }

        return null;
    }

    public void RegisterAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            CurrentScore++;
            UpdateScoreText();
        }
        questionsAnswered++;
    }

    // Utility to load questions from JSON file in StreamingAssets
    public List<TriviaQuestion> LoadFromJSON()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "trivia_questions.json");

        if (!File.Exists(path))
        {
            Debug.LogError("[TriviaManager] JSON file not found at: " + path);
            return new List<TriviaQuestion>();
        }

        string json = File.ReadAllText(path);
        return new List<TriviaQuestion>(JsonHelper.FromJson<TriviaQuestion>(json));
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {CurrentScore}";
        PlayerPrefs.SetInt("TriviaScore", CurrentScore);
        PlayerPrefs.Save();
    }
} 

// TriviaUIController.cs — no change needed if you already call GetNextQuestion() correctly
    



// TriviaUIController.cs — no change needed if you already call GetNextQuestion() correctly

// TriviaUIController.cs — no change needed if you already call GetNextQuestion() correctly