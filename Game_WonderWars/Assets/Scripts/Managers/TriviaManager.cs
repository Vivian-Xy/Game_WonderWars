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

    [Header("End-Of-Quiz UI")]
    public GameObject endQuizPanel;        // drag EndQuizPanel here
    public TextMeshProUGUI finalScoreText; // drag FinalScoreText here

    [Header("Reward Settings")]
    public GameObject rewardPrefab;
    public Transform rewardSpawnPoint;

    [Header("Trivia Data")]
    public List<TriviaQuestion> questions = new List<TriviaQuestion>();
    public int currentQuestionIndex = 0;
    public int CurrentScore = 0;

    [Header("Audio & Haptics")]
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioSource audioSource;

    // Haptic amplitude and duration
    [Range(0f, 1f)] public float hapticAmplitude = 0.5f;
    public float hapticDuration = 0.1f;

    void Awake()
    {
        //Get or add an AudioSource on this GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Load score from PlayerPrefs
        CurrentScore = PlayerPrefs.GetInt("TriviaScore", 0);
    }
    void Start()
    {
        if (questions != null && questions.Count > 0)
        {
            UpdateScoreText();
            DisplayQuestion();
        }
        else
        {
            Debug.LogWarning("TriviaManager: No questions have been added!");
        }
    }

    /// <summary>
    /// Populates UI from the current TriviaQuestion.
    /// </summary>
    public void DisplayQuestion()
    {
        // Clear previous feedback
        if (feedbackText != null)
            feedbackText.text = "";

        // Safety checks:
        if (questions == null || questions.Count == 0)
        {
            Debug.LogWarning("TriviaManager: No questions available to display.");
            return;
        }
        if (currentQuestionIndex < 0) currentQuestionIndex = 0;
        if (currentQuestionIndex >= questions.Count) currentQuestionIndex = questions.Count - 1;

        // Grab the current question
        TriviaQuestion current = questions[currentQuestionIndex];

        // Update question text
        questionText.text = current.questionText;

        // Loop through each UI button and set its label + listener
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < current.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                TextMeshProUGUI btnText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                btnText.text = current.answers[i];

                // Remove old listeners
                answerButtons[i].onClick.RemoveAllListeners();

                // Capture 'i' in a local variable for the lambda
                int indexCopy = i;
                answerButtons[i].onClick.AddListener(() =>
                {
                    CheckAnswer(indexCopy);
                });
            }
            else
            {
                // Hide any extra buttons
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Called by a button click, passing in the index of the chosen answer.
    /// Spawns reward if correct, advances to next question, or finishes quiz.
    /// </summary>
    public void CheckAnswer(int chosenIndex)
    {
        TriviaQuestion current = questions[currentQuestionIndex];
        bool isCorrect = (chosenIndex == current.correctAnswerIndex);

        if (isCorrect)
        {
            CurrentScore++;
            UpdateScoreText();
            // Correct answer actions
            Debug.Log($"Question {currentQuestionIndex + 1}: Correct!");
            if (feedbackText != null)
                feedbackText.text = "Correct!";

            if (audioSource != null && correctClip != null)
                audioSource.PlayOneShot(correctClip);
            //Haptic on both controllers (or just the active controller)
            SendHapticImpulse(hapticAmplitude, hapticDuration);

            StartCoroutine(FlashButtonColor(answerButtons[chosenIndex], Color.green, 0.3f));
            StartCoroutine(PopButtonAnimation(answerButtons[chosenIndex].transform));

            if (rewardPrefab != null && rewardSpawnPoint != null)
            {
                Instantiate(rewardPrefab, rewardSpawnPoint.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log($"Question {currentQuestionIndex + 1}: Wrong. You chose \"{current.answers[chosenIndex]}\"");
            if (feedbackText != null)
                feedbackText.text = "Wrong—try again!";
            if (audioSource != null && wrongClip != null)
                audioSource.PlayOneShot(wrongClip);

            SendHapticImpulse(hapticAmplitude * 0.5f, hapticDuration); // softer vibration

            StartCoroutine(FlashButtonColor(answerButtons[chosenIndex], Color.red, 0.3f));
            StartCoroutine(PopButtonAnimation(answerButtons[chosenIndex].transform));

        }

        // Advance index
        currentQuestionIndex++;

        // If more questions remain, display next; otherwise end or restart
        if (currentQuestionIndex < questions.Count)
        {
            DisplayQuestion();
        }
        else
        {
            Debug.Log("Trivia complete! No more questions.");
            // Option A: Disable all buttons
            for (int i = 0; i < answerButtons.Length; i++)
                answerButtons[i].gameObject.SetActive(false);

            // Option B (uncomment to restart automatically):
            // currentQuestionIndex = 0;
            // DisplayQuestion();
            // Update and show the EndQuizPanel
            if (finalScoreText != null)
            {
                finalScoreText.text = "Your Score: " + CurrentScore;
            }

            if (endQuizPanel != null)
            {
                endQuizPanel.SetActive(true);
            }
        }
    }
    /// <summary>
    /// Sends a haptic impulse to the left and right controllers, if they exist.
    /// </summary>
    private void SendHapticImpulse(float amplitude, float duration)
    {
        // Find all XRController components under the XR Origin
        XRController[] controllers = FindObjectsOfType<XRController>();
        foreach (var ctrl in controllers)
        {
            ctrl.SendHapticImpulse(amplitude, duration);
        }
    }

    // Add a helper method for score text
    private void SetScoreText(string text)
    {
        if (scoreText != null)
            scoreText.text = text;
    }

    private void UpdateScoreText()
    {
        SetScoreText($"Score: {CurrentScore}");
        // Save score to PlayerPrefs
        PlayerPrefs.SetInt("TriviaScore", CurrentScore);
        PlayerPrefs.Save();
    }

    // Optional: Call this to reset the saved score
    public void ResetScore()
    {
        CurrentScore = 0;
        PlayerPrefs.SetInt("TriviaScore", 0);
        PlayerPrefs.Save();
        UpdateScoreText();
    }

    // Coroutine for simple button pop/scale animation
    private IEnumerator PopButtonAnimation(Transform buttonTransform)
    {
        Vector3 originalScale = buttonTransform.localScale;
        Vector3 popScale = originalScale * 1.15f;
        float duration = 0.1f;
        float t = 0f;

        // Scale up
        while (t < duration)
        {
            buttonTransform.localScale = Vector3.Lerp(originalScale, popScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        buttonTransform.localScale = popScale;

        // Scale down
        t = 0f;
        while (t < duration)
        {
            buttonTransform.localScale = Vector3.Lerp(popScale, originalScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        buttonTransform.localScale = originalScale;
    }

    // Add missing FlashButtonColor coroutine if not present
    private IEnumerator FlashButtonColor(Button button, Color color, float duration)
    {
        var colors = button.colors;
        Color originalColor = colors.normalColor;
        colors.normalColor = color;
        button.colors = colors;
        yield return new WaitForSeconds(duration);
        colors.normalColor = originalColor;
        button.colors = colors;
    }
    /// <summary>
    /// Called when the player presses the “Restart Quiz” button.
    /// Resets score, question index, hides the EndQuizPanel, re‐enables buttons, and shows the first question.
    /// </summary>
    public void RestartQuiz()
    {
        // 1) Reset the score and update UI
        CurrentScore = 0;
        UpdateScoreText();

        // 2) Reset question index
        currentQuestionIndex = 0;

        // 3) Hide the EndQuizPanel
        if (endQuizPanel != null)
            endQuizPanel.SetActive(false);

        // 4) Re-enable all answer buttons (in case any were hidden)
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].gameObject.SetActive(true);

        // 5) Display the first question again
        DisplayQuestion();
    }
}