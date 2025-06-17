using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class TriviaManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI feedbackText;    // Optional feedback
    public TextMeshProUGUI scoreText;
    public Animator feedbackAnimator; // <-- Add here, after other UI references

    [Header("Reward Settings")]
    public List<GameObject> rewardPrefab;
    public Transform rewardSpawnPoint;

    [Header("Trivia Data")]
    public List<TriviaQuestion> questions = new List<TriviaQuestion>();
    public int currentQuestionIndex = 0;
    public int CurrentScore = 0;

    // Add a private pool for shuffled questions
    private List<TriviaQuestion> pool;

    [Header("Audio & Haptics")]
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioSource audioSource;

    [Header("Visual FX")]
    public GameObject correctFXPrefab;
    public GameObject wrongFXPrefab;
    public float fxSpawnYOffset = 0.5f;

    [Range(0f, 1f)] public float hapticAmplitude = 0.5f;
    public float hapticDuration = 0.1f;

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
        // 1) Copy the master list into our working pool:
        pool = new List<TriviaQuestion>(questions);

        // 2) Shuffle so questions appear in random order:
        Shuffle(pool);

        // 3) Reset index & score display:
        currentQuestionIndex = 0;
        UpdateScoreText();

        // 4) Kick off the first question:
        if (pool.Count > 0)
            DisplayQuestion();
        else
            Debug.LogWarning("TriviaManager: No questions have been added!");
    }


    public void DisplayQuestion()
    {
        if (feedbackText != null)
            feedbackText.text = "";

        if (pool == null || pool.Count == 0)
        {
            Debug.LogWarning("TriviaManager: No questions available to display.");
            return;
        }
        if (currentQuestionIndex < 0) currentQuestionIndex = 0;
        if (currentQuestionIndex >= pool.Count) currentQuestionIndex = pool.Count - 1;

        TriviaQuestion current = pool[currentQuestionIndex];
        questionText.text = current.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < current.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                TextMeshProUGUI btnText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                btnText.text = current.answers[i];
                answerButtons[i].onClick.RemoveAllListeners();
                int indexCopy = i;
                answerButtons[i].onClick.AddListener(() => CheckAnswer(indexCopy));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void CheckAnswer(int chosenIndex)
    {
        if (pool == null || pool.Count == 0 || currentQuestionIndex < 0 || currentQuestionIndex >= pool.Count)
            return;

        TriviaQuestion current = pool[currentQuestionIndex];
        bool isCorrect = (chosenIndex == current.correctAnswerIndex);

        if (isCorrect)
        {
            CurrentScore++;
            UpdateScoreText();
            Debug.Log($"Question {currentQuestionIndex + 1}: Correct!");
            if (feedbackText != null)
                feedbackText.text = "Correct!";

            // Play feedback animation if animator is assigned
            if (feedbackAnimator != null)
                feedbackAnimator.Play("FeedbackAnim", -1, 0f);

            if (audioSource != null && correctClip != null)
                audioSource.PlayOneShot(correctClip);

            SendHapticImpulse(hapticAmplitude, hapticDuration);

            StartCoroutine(FlashButtonColor(answerButtons[chosenIndex], Color.green, 0.3f));
            StartCoroutine(PopButtonAnimation(answerButtons[chosenIndex].transform));

            // FX spawn position just above the selected button:
            Vector3 btnPos = answerButtons[chosenIndex].transform.position;
            Vector3 fxPos = btnPos + Vector3.up * fxSpawnYOffset;
            Instantiate(correctFXPrefab, fxPos, Quaternion.identity);

            // --- REWARD INSTANTIATION LOGIC ---
            if (rewardSpawnPoint != null)
            {
                // Defensive: Check if rewardPrefabID exists
                string rewardID = null;
                // Try to get rewardPrefabID property (if it exists)
                try
                {
                    rewardID = (string)current.GetType().GetField("rewardPrefabID").GetValue(current);
                }
                catch
                {
                    Debug.LogWarning("TriviaQuestion does not have a rewardPrefabID field.");
                }

                if (!string.IsNullOrEmpty(rewardID))
                {
                    GameObject prefab = GetRewardPrefab(rewardID);
                    if (prefab != null)
                    {
                        var instance = Instantiate(prefab,
                                                   rewardSpawnPoint.position,
                                                   Quaternion.identity);

                        var grab = instance.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                        if (grab == null)
                            instance.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

                        instance.transform.SetParent(rewardSpawnPoint.parent, true);
                    }
                }
                else
                {
                    Debug.LogWarning("No rewardPrefabID specified for this question.");
                }
            }
            // --- END REWARD INSTANTIATION LOGIC ---

            // Pause before showing next question
            StartCoroutine(NextQuestionAfterDelay(1.0f));
            return; // Prevent immediate advancement
        }
        else
        {
            Debug.Log($"Question {currentQuestionIndex + 1}: Wrong. You chose \"{current.answers[chosenIndex]}\"");
            if (feedbackText != null)
                feedbackText.text = "Wrong—try again!";

            // Play feedback animation if animator is assigned
            if (feedbackAnimator != null)
                feedbackAnimator.Play("FeedbackAnim", -1, 0f);

            if (audioSource != null && wrongClip != null)
                audioSource.PlayOneShot(wrongClip);

            SendHapticImpulse(hapticAmplitude * 0.5f, hapticDuration);

            StartCoroutine(FlashButtonColor(answerButtons[chosenIndex], Color.red, 0.3f));
            StartCoroutine(PopButtonAnimation(answerButtons[chosenIndex].transform));

            // FX spawn position just above the selected button for wrong answer:
            Vector3 fxPos = answerButtons[chosenIndex].transform.position + Vector3.up * fxSpawnYOffset;
            Instantiate(wrongFXPrefab, fxPos, Quaternion.identity);

            // Immediate advancement for wrong answers (if desired)
            currentQuestionIndex++;
            if (currentQuestionIndex < pool.Count)
            {
                DisplayQuestion();
            }
            else
            {
                EndTrivia();
            }
        }
    }

    private void SendHapticImpulse(float amplitude, float duration)
    {
        XRController[] controllers = FindObjectsOfType<XRController>();
        foreach (var ctrl in controllers)
        {
            try
            {
                ctrl.SendHapticImpulse(amplitude, duration);
            }
            catch
            {
                // Ignore if not supported
            }
        }
    }

    private void SetScoreText(string text)
    {
        if (scoreText != null)
            scoreText.text = text;
    }

    private void UpdateScoreText()
    {
        SetScoreText($"Score: {CurrentScore}");
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

    private IEnumerator PopButtonAnimation(Transform buttonTransform)
    {
        Vector3 originalScale = buttonTransform.localScale;
        Vector3 popScale = originalScale * 1.15f;
        float duration = 0.1f;
        float t = 0f;
        while (t < duration)
        {
            buttonTransform.localScale = Vector3.Lerp(originalScale, popScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        buttonTransform.localScale = popScale;
        t = 0f;
        while (t < duration)
        {
            buttonTransform.localScale = Vector3.Lerp(popScale, originalScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        buttonTransform.localScale = originalScale;
    }

    private IEnumerator FlashButtonColor(Button button, Color flashColor, float duration)
    {
        var colors = button.colors;
        Color originalNormal = colors.normalColor;
        Color originalHighlighted = colors.highlightedColor;

        colors.normalColor = flashColor;
        colors.highlightedColor = flashColor;
        button.colors = colors;

        yield return new WaitForSeconds(duration);

        colors.normalColor = originalNormal;
        colors.highlightedColor = originalHighlighted;
        button.colors = colors;
    }

    /// <summary>
    /// Randomizes the list in place.
    /// </summary>
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
    /// <summary>
    /// Looks up the prefab by matching its name to rewardID.
    /// </summary>
    private GameObject GetRewardPrefab(string rewardID)
    {
        if (string.IsNullOrEmpty(rewardID))
            return null;
        foreach (var prefab in rewardPrefab)
        {
            if (prefab != null && prefab.name == rewardID)
                return prefab;
        }
        Debug.LogWarning($"Reward prefab for ID '{rewardID}' not found!");
        return null;
    }

    private IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentQuestionIndex++;
        if (currentQuestionIndex < pool.Count)
            DisplayQuestion();
        else
            EndTrivia();
    }

    private void EndTrivia()
    {
        Debug.Log("All done!");
        // Hide buttons / show “Complete” UI
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].gameObject.SetActive(false);
        // Optionally show a "Complete" message or UI here
    }
}