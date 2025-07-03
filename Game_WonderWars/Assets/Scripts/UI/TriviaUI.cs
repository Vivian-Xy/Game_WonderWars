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
    public Button[] answerButtons;
    public GameObject progressCard;

    private QuestionData currentQuestion;
    private int questionsAnswered = 0;
    private const int MaxQuestions = 20;

    void Start()
    {
        questionsAnswered = 0;
        ShowNextQuestion();
    }

    public void ShowNextQuestion()
    {
        var question = TriviaManager.Instance.GetNextQuestion();

        // Only check for null, let TriviaManager handle MaxQuestions logic
        if (question == null)
        {
            Debug.Log("[TriviaUI] No more questions, going back to Dashboard");
            SceneManager.LoadScene("DashboardScene");
            return;
        }

        currentQuestion = new QuestionData
        {
            questionText = question.questionText,
            answers = question.answers,
            correctAnswerIndex = question.correctAnswerIndex,
            RewardPrefabID = question.rewardPrefabID
        };

        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < currentQuestion.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i];
                int index = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        if (progressCard != null)
            progressCard.SetActive(false);
    }

    public void OnChoiceSelected(int index)
    {
        if (index < 0 || index >= currentQuestion.answers.Length)
        {
            Debug.LogError($"[TriviaUI] Invalid index selected: {index}");
            return;
        }

        bool isCorrect = index == currentQuestion.correctAnswerIndex;

        if (isCorrect)
            RewardPlayer(currentQuestion.RewardPrefabID);

        HighlightAnswers(index);

        // Update score and question count in TriviaManager
        TriviaManager.Instance.RegisterAnswer(isCorrect);

        StartCoroutine(ProceedAfterDelay(1.5f));
    }

    IEnumerator ProceedAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // Only check if there are more questions (let TriviaManager handle max logic)
        // Remove duplicate SceneManager.LoadScene("DashboardScene") calls elsewhere in this script.
        if (TriviaManager.Instance.GetNextQuestion() == null)
        {
            SceneManager.LoadScene("DashboardScene");
        }
        else
        {
            ShowNextQuestion();
        }
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

    void RewardPlayer(string piece)
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.AddPiece(piece);
        }

        if (progressCard != null)
        {
            progressCard.SetActive(true);
        }
    }
}