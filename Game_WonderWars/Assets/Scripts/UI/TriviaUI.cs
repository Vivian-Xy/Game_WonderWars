using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TriviaUI : MonoBehaviour
{
    [Header("UI Refs")]
    public TMP_Text questionText;
    public List<Button> optionButtons; // size 4
    public TMP_Text feedbackText;
    public Button nextButton;

    [Header("Backend")]
    public BackendManager backend;

    private List<BackendManager.Question> questions;
    private int currentIndex = 0;
    private string userId;

    void Start()
    {
        // Check for StreamingAssets JSON file (for debugging only; not used by backend)
        string path = Path.Combine(Application.streamingAssetsPath, "trivia_questions.json");
        Debug.Log("Questions file path: " + path);
        if (!File.Exists(path))
            Debug.LogWarning("JSON not found in StreamingAssets! (This is only needed if you load questions locally)");

        userId = PlayerPrefs.GetString("userId");
        feedbackText.text = "";
        nextButton.interactable = false;
        nextButton.onClick.AddListener(LoadNextQuestion);

        StartCoroutine(backend.GetQuestions(OnQuestionsLoaded));
    }

    void OnQuestionsLoaded(List<BackendManager.Question> list)
    {
        if (list == null || list.Count == 0)
        {
            feedbackText.text = "No questions available.";
            foreach (var btn in optionButtons)
                btn.gameObject.SetActive(false);
            nextButton.interactable = false;
            return;
        }
        questions = list;
        ShowQuestion(0);
    }

    void ShowQuestion(int index)
    {
        if (questions == null || questions.Count == 0 || index < 0 || index >= questions.Count)
        {
            feedbackText.text = "No more questions.";
            foreach (var btn in optionButtons)
                btn.gameObject.SetActive(false);
            nextButton.interactable = false;
            return;
        }

        currentIndex = index;
        feedbackText.text = "";
        nextButton.interactable = false;

        var q = questions[index];
        questionText.text = q.questionText;

        for (int i = 0; i < optionButtons.Count; i++)
        {
            var btn = optionButtons[i];
            if (q.options != null && i < q.options.Count)
            {
                btn.gameObject.SetActive(true);
                var txt = btn.GetComponentInChildren<TMP_Text>();
                if (txt != null)
                    txt.text = q.options[i];
                btn.onClick.RemoveAllListeners();
                int choice = i;
                btn.onClick.AddListener(() => OnOptionSelected(q, choice));
                btn.interactable = true;
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }
    }

    void OnOptionSelected(BackendManager.Question q, int choice)
    {
        string selected = q.options[choice];
        bool correct = selected == q.correctAnswer;
        feedbackText.text = correct ? "Correct!" : "Wrong!";
        foreach (var btn in optionButtons) btn.interactable = false;

        if (correct)
        {
            StartCoroutine(backend.UpdateProgress(
                userId, q.id, q.monumentUnlocked, 10, OnProgressUpdated
            ));
        }

        nextButton.interactable = true;
    }

    void OnProgressUpdated(bool success, string resp)
    {
        if (!success) Debug.LogError("Progress update failed: " + resp);
    }

    void LoadNextQuestion()
    {
        int next = currentIndex + 1;
        if (next < questions.Count)
            ShowQuestion(next);
        else
            feedbackText.text = "All done!";
    }
}