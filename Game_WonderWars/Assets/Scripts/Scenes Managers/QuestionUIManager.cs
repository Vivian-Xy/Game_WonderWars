using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json; // Add this for Newtonsoft.Json support

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    private List<QuestionData> allQuestions;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadQuestions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadQuestions()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "trivia_questions.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            try
            {
                allQuestions = JsonConvert.DeserializeObject<List<QuestionData>>(json);
                Debug.Log($"Loaded {allQuestions.Count} questions.");
                // Debug log the first question's answers for troubleshooting
                if (allQuestions.Count > 0)
                {
                    var q = allQuestions[0];
                    Debug.Log($"First question: {q.questionText}");
                    if (q.answers != null)
                        Debug.Log($"Answers: {string.Join(", ", q.answers)}");
                    else
                        Debug.LogWarning("First question's answers field is null.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to parse trivia_questions.json: " + ex.Message);
                allQuestions = new List<QuestionData>();
            }
        }
        else
        {
            Debug.LogError("Could not find trivia_questions.json at: " + filePath);
        }
    }

    public QuestionData GetRandomQuestion()
    {
        if (allQuestions == null || allQuestions.Count == 0)
        {
            Debug.LogWarning("No questions loaded.");
            return null;
        }

        int index = Random.Range(0, allQuestions.Count);
        return allQuestions[index];
    }
}