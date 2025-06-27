using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestionUIManager : MonoBehaviour
{
    public Text questionText;
    public List<Button> optionButtons;
    private BackendManager.Question currentQuestion;
    private BackendManager backend;
    private string userId = "USER_ID_FROM_LOGIN";

    void Awake()
    {
        backend = FindObjectOfType<BackendManager>();
        if (backend == null)
            Debug.LogError("BackendManager not found in scene!");
    }

    public void DisplayQuestion(BackendManager.Question question)
    {
        currentQuestion = question;
        questionText.text = question.questionText;

        for (int i = 0; i < optionButtons.Count; i++)
        {
            int index = i;
            if (i < question.options.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<Text>().text = question.options[i];
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnOptionSelected(int index)
    {
        string selected = currentQuestion.options[index];
        bool isCorrect = selected == currentQuestion.correctAnswer;

        Debug.Log(isCorrect ? "✅ Correct!" : "❌ Wrong");

        if (isCorrect)
        {
            // If monumentUnlocked is not present, add it to BackendManager.Question and your backend
            string monumentId = "";
            if (currentQuestion.GetType().GetField("monumentUnlocked") != null)
                monumentId = (string)currentQuestion.GetType().GetField("monumentUnlocked").GetValue(currentQuestion);

            StartCoroutine(backend.UpdateProgress(userId, currentQuestion.questionText, monumentId, 10));
            // Show monument in game or continue
        }
    }
}