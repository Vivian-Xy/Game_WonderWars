using UnityEngine;
using System.IO;
using System.Collections.Generic;
#if USE_NEWTONSOFT
using Newtonsoft.Json;
#endif

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }
    public List<TriviaQuestion> allQuestions;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadQuestions();
    }

    void LoadQuestions()
    {
        string fileName = "trivia_questions.json";
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"[QuestionManager] File not found: {fullPath}");
            allQuestions = new List<TriviaQuestion>();
            return;
        }

        string json = File.ReadAllText(fullPath);

        // Deserialize
    #if USE_NEWTONSOFT
        allQuestions = JsonConvert.DeserializeObject<List<TriviaQuestion>>(json);
    #else
        // If you didn’t install Newtonsoft, wrap your JSON in {"questions": [...]}
        try
        {
            var wrapper = JsonUtility.FromJson<TriviaQuestionList>(json);
            allQuestions = wrapper != null && wrapper.questions != null
                ? new List<TriviaQuestion>(wrapper.questions)
                : new List<TriviaQuestion>();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[QuestionManager] JSON parse error: {ex.Message}");
            allQuestions = new List<TriviaQuestion>();
        }
    #endif

        Debug.Log($"[QuestionManager] Loaded {allQuestions.Count} questions.");
    }

    /// <summary>
    /// Returns a random question from the list.
    /// </summary>
    public TriviaQuestion GetRandomQuestion()
    {
        if (allQuestions == null || allQuestions.Count == 0)
        {
            Debug.LogWarning("[QuestionManager] No questions loaded.");
            return null;
        }
        int index = Random.Range(0, allQuestions.Count);
        return allQuestions[index];
    }
}

// Only needed if not using Newtonsoft
[System.Serializable]
public class TriviaQuestionList
{
    public TriviaQuestion[] questions;
}