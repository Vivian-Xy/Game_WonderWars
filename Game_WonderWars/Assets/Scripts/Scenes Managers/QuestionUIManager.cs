using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }
    public List<QuestionData> allQuestions;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        StartCoroutine(LoadQuestions());
    }

    IEnumerator LoadQuestions()
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "trivia_questions.json");

#if UNITY_ANDROID && !UNITY_EDITOR
        // On Android, StreamingAssets is inside the APK and must use "jar:file://" + Application.dataPath + "!/assets/"
        string uri = Application.streamingAssetsPath + "/trivia_questions.json";
        if (!uri.StartsWith("jar:"))
            uri = "jar:file://" + uri;
#else
        string uri = "file://" + filePath;
#endif

        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load JSON: " + www.error);
            allQuestions = new List<QuestionData>();
        }
        else
        {
            string json = www.downloadHandler.text;
            allQuestions = JsonUtilityWrapper.FromJson<QuestionData>(json);
            if (allQuestions == null)
                allQuestions = new List<QuestionData>();
            Debug.Log($"Loaded {allQuestions.Count} questions.");
            // Debug: print first question and answers
            if (allQuestions.Count > 0)
            {
                Debug.Log($"First Q: {allQuestions[0].questionText}");
                if (allQuestions[0].answers != null)
                    Debug.Log($"Answers: {string.Join(", ", allQuestions[0].answers)}");
                else
                    Debug.LogWarning("First question's answers field is null.");
            }
        }
    }

    public QuestionData GetRandomQuestion()
    {
        if (allQuestions == null || allQuestions.Count == 0)
        {
            Debug.LogWarning("No questions loaded.");
            return null;
        }
        int idx = Random.Range(0, allQuestions.Count);
        var q = allQuestions[idx];
        allQuestions.RemoveAt(idx);
        return q;
    }
}