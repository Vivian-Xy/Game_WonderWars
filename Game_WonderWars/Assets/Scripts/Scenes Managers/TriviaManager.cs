using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Manages trivia questions, user input, rewards, and history tracking.
/// </summary>
public class TriviaManager : MonoBehaviour
{
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

    // Working pool after shuffle
    private List<TriviaQuestion> pool;
    private int currentQuestionIndex = 0;

    public int CurrentScore = 0;

    // Runtime tracking
    // Exposed for the Dashboard
    [System.Serializable]
    public class TriviaHistoryItem
    {
        public string questionText;
        public bool wasCorrect;
        public string rewardID; // which monument, if any
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
        public int totalPossible;   // usually number of questions for that monument
    }

    public List<TriviaHistoryItem> history = new List<TriviaHistoryItem>();
    public List<MonumentProgress> monumentProgress = new List<MonumentProgress>();

    #region Data Classes
    #endregion

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Load score from PlayerPrefs
        CurrentScore = PlayerPrefs.GetInt("TriviaScore", 0);
    }

    void Start()
    {
        // Prepare question pool
        pool = new List<TriviaQuestion>(questions);
        Shuffle(pool);

        // Reset index and score UI
        currentQuestionIndex = 0;
        UpdateScoreText();

        // Initialize monument progress based on question bank
        InitializeMonumentProgress();

        // Display first question
        if (pool.Count > 0)
            DisplayQuestion();
        else
            Debug.LogWarning("TriviaManager: No questions available.");
    }

    #region Initialization
    private void InitializeMonumentProgress()
    {
        monumentProgress.Clear();
        // For each unique rewardID in questions
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

        // Restore unlockedCount from PlayerPrefs
        foreach (var mp in monumentProgress)
            mp.unlockedCount = PlayerPrefs.GetInt($"Unlocked_{mp.monumentID}", 0);
    }
    #endregion

    #region Question Flow
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

    public void DisplayQuestion()
    {
        if (feedbackText != null)
            feedbackText.text = "";

        if (currentQuestionIndex >= pool.Count)
        {
            EndTrivia();
            return;
        }

        var current = pool[currentQuestionIndex];
        questionText.text = current.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            var btn = answerButtons[i];
            if (i < current.answers.Length)
            {
                btn.gameObject.SetActive(true);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = current.answers[i];
                btn.onClick.RemoveAllListeners();
                int idx = i;
                btn.onClick.AddListener(() => CheckAnswer(idx));
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }
    }

    public void CheckAnswer(int chosenIndex)
    {
        var current = pool[currentQuestionIndex];
        bool isCorrect = (chosenIndex == current.correctAnswerIndex);
        string rewardID = current.rewardPrefabID;

        // Record history
        history.Add(new TriviaHistoryItem(
            current.questionText, isCorrect, rewardID));

        if (isCorrect)
        {
            CurrentScore++;
            UpdateScoreText();
            if (feedbackText != null)
                feedbackText.text = "Correct!";
            if (correctClip != null)
                SFXManager.I.Play(correctClip, 0.8f);

            SendHapticImpulse(hapticAmplitude, hapticDuration);
            StartCoroutine(FlashButtonColor(answerButtons[chosenIndex], Color.green, 0.3f));
            StartCoroutine(PopButtonAnimation(answerButtons[chosenIndex].transform));

            // Spawn FX
            SpawnFX(correctFXPrefab, answerButtons[chosenIndex].transform.position);

            // Spawn reward prefab
            var prefab = GetRewardPrefab(rewardID);
            if (prefab != null && rewardSpawnPoint != null)
            {
                var inst = Instantiate(prefab, rewardSpawnPoint.position, Quaternion.identity);
                inst.transform.SetParent(rewardSpawnPoint.parent, true);
                if (inst.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() == null)
                    inst.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

                // Play reward spawn SFX
                if (rewardSpawnClip != null)
                    SFXManager.I.Play(rewardSpawnClip, 0.8f);

                // Update progress
                var mp = monumentProgress.First(m => m.monumentID == rewardID);
                mp.unlockedCount++;
                PlayerPrefs.SetInt($"Unlocked_{mp.monumentID}", mp.unlockedCount);
            }
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Wrong—try again!";
            if (wrongClip != null)
                SFXManager.I.Play(wrongClip, 0.8f);

            SendHapticImpulse(hapticAmplitude * 0.5f, hapticDuration);
            StartCoroutine(FlashButtonColor(answerButtons[chosenIndex], Color.red, 0.3f));
            StartCoroutine(PopButtonAnimation(answerButtons[chosenIndex].transform));

            // Spawn FX
            SpawnFX(wrongFXPrefab, answerButtons[chosenIndex].transform.position);
        }

        currentQuestionIndex++;
        StartCoroutine(NextQuestionAfterDelay(1f));
    }

    private IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayQuestion();
    }

    private void EndTrivia()
    {
        Debug.Log("Trivia complete! No more questions.");
        foreach (var btn in answerButtons)
            btn.gameObject.SetActive(false);
            
        // Show the Continue button
        var continueBtn = GameObject.Find("TriviaCanvas/ContinueButton");
        if (continueBtn != null)
        continueBtn.SetActive(true);
    }

    private void SpawnFX(GameObject fxPrefab, Vector3 position)
    {
        if (fxPrefab == null) return;
        Vector3 fxPos = position + Vector3.up * fxSpawnYOffset;
        Instantiate(fxPrefab, fxPos, Quaternion.identity);
    }
    #endregion

    #region Helpers
    private GameObject GetRewardPrefab(string rewardID)
    {
        foreach (var prefab in rewardPrefabs)
        {
            if (prefab.name == rewardID)
                return prefab;
        }
        Debug.LogWarning($"Reward prefab for ID '{rewardID}' not found!");
        return null;
    }

    private void SendHapticImpulse(float amplitude, float duration)
    {
        var controllers = FindObjectsOfType<XRController>();
        foreach (var ctrl in controllers)
        {
            try { ctrl.SendHapticImpulse(amplitude, duration); } catch { }
        }
    }

    private IEnumerator PopButtonAnimation(Transform buttonTransform)
    {
        Vector3 orig = buttonTransform.localScale;
        Vector3 pop = orig * 1.15f;
        float t = 0f, dur = 0.1f;
        while (t < dur)
        {
            buttonTransform.localScale = Vector3.Lerp(orig, pop, t / dur);
            t += Time.deltaTime;
            yield return null;
        }
        buttonTransform.localScale = pop;
        t = 0f;
        while (t < dur)
        {
            buttonTransform.localScale = Vector3.Lerp(pop, orig, t / dur);
            t += Time.deltaTime;
            yield return null;
        }
        buttonTransform.localScale = orig;
    }

    private IEnumerator FlashButtonColor(Button button, Color flashColor, float duration)
    {
        var colors = button.colors;
        Color origNorm = colors.normalColor;
        Color origHigh = colors.highlightedColor;
        colors.normalColor = flashColor;
        colors.highlightedColor = flashColor;
        button.colors = colors;
        yield return new WaitForSeconds(duration);
        colors.normalColor = origNorm;
        colors.highlightedColor = origHigh;
        button.colors = colors;
    }
    #endregion

    #region Score Persistence
    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {CurrentScore}";
        PlayerPrefs.SetInt("TriviaScore", CurrentScore);
        PlayerPrefs.Save();
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        PlayerPrefs.SetInt("TriviaScore", 0);
        PlayerPrefs.Save();
        UpdateScoreText();
    }
    #endregion
}