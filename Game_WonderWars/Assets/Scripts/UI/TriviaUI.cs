using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TriviaUIController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public Button[] answerButtons; // Assign AnswerButton1–4 in Inspector
    public GameObject progressCard; // Optional reward panel

    private QuestionData currentQuestion;

    void Start()
    {
        // Wait for QuestionManager.Instance to be assigned (not just null check)
        StartCoroutine(WaitForQuestionManager());
    }

    private IEnumerator WaitForQuestionManager()
    {
        int safety = 0;
        // Wait until the singleton is assigned (Awake runs on all objects before Start)
        while (QuestionManager.Instance == null && safety < 300)
        {
            yield return null;
            safety++;
        }

        if (QuestionManager.Instance == null)
        {
            Debug.LogError("TriviaUIController: QuestionManager.Instance is STILL null after waiting.");
            questionText.text = "Question system not initialized.";
            foreach (var btn in answerButtons)
                btn.gameObject.SetActive(false);
            yield break;
        }

        // Now wait for questions to load
        yield return StartCoroutine(DelayedShow());
    }

    IEnumerator DelayedShow()
    {
        // Wait for questions to load
        int safety = 0;
        while ((QuestionManager.Instance.allQuestions == null || QuestionManager.Instance.allQuestions.Count == 0) && safety < 300)
        {
            yield return null;
            safety++;
        }

        if (QuestionManager.Instance.allQuestions == null || QuestionManager.Instance.allQuestions.Count == 0)
        {
            Debug.LogError("TriviaUIController: Questions failed to load after waiting.");
            questionText.text = "Failed to load questions.";
            foreach (var btn in answerButtons)
                btn.gameObject.SetActive(false);
            yield break;
        }

        ShowNextQuestion();
    }

    public void ShowNextQuestion()
    {
        currentQuestion = QuestionManager.Instance.GetRandomQuestion();

        if (currentQuestion == null)
        {
            Debug.LogWarning("No question available.");
            questionText.text = "No question available.";
            foreach (var btn in answerButtons)
                btn.gameObject.SetActive(false);
            return;
        }

        questionText.text = currentQuestion.questionText;
        Debug.Log($"Loaded Question: {currentQuestion.questionText}");

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            if (currentQuestion.answers != null && currentQuestion.answers.Length > index)
            {
                answerButtons[i].gameObject.SetActive(true);
                TextMeshProUGUI label = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = currentQuestion.answers[index];
                    Debug.Log($"Set AnswerButton {i + 1}: {label.text}");
                }
                else
                {
                    Debug.LogWarning($"AnswerButton {i + 1} has no TextMeshProUGUI child!");
                }
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
                answerButtons[i].interactable = true;
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnChoiceSelected(int selectedIndex)
    {
        // Defensive: check answers list and index
        bool isCorrect = false;
        if (currentQuestion.answers != null && selectedIndex >= 0 && selectedIndex < currentQuestion.answers.Length)
        {
            isCorrect = string.Equals(
                currentQuestion.answers[selectedIndex].Trim(),
                currentQuestion.answers[currentQuestion.correctAnswerIndex].Trim(),
                System.StringComparison.OrdinalIgnoreCase
            );
        }
        else
        {
            Debug.LogWarning($"Selected answer index {selectedIndex} is out of range or answers list is null.");
        }

        Debug.Log($"You selected {selectedIndex} → {(isCorrect ? "Correct" : "Wrong")}");

        // Highlight the selected button (optional visual feedback)
        HighlightAnswers(selectedIndex);

        // If correct, reward
        if (isCorrect)
        {
            RewardPlayer(currentQuestion.RewardPrefabID);
        }

        // Go back to dashboard after short delay
        StartCoroutine(ReturnToDashboardAfterDelay(1.5f));
    }

    void HighlightAnswers(int selectedIndex)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            var colors = answerButtons[i].colors;
            if (i == currentQuestion.correctAnswerIndex)
                colors.normalColor = Color.green;
            else if (i == selectedIndex)
                colors.normalColor = Color.red;
            else
                colors.normalColor = Color.white;

            answerButtons[i].colors = colors;
        }
    }

    void RewardPlayer(string pieceName)
    {
        Debug.Log($"🎁 Player earned: {pieceName}");
        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.AddPiece(pieceName);

        if (progressCard != null)
        {
            progressCard.SetActive(true);
        }
    }

    IEnumerator ReturnToDashboardAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene("DashboardScene"); // Adjust scene name if needed
    }

    // Optional manual back button
    public void BackToDashboard()
    {
        SceneManager.LoadScene("DashboardScene");
    }
}