using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Add the correct namespace for QuestionUIManager below if it exists, for example:
// using WonderWars.UI; // Uncomment and replace with the actual namespace if needed
// using WonderWars.UI; // Replace 'WonderWars.UI' with the actual namespace of QuestionUIManager if different

// If QuestionUIManager is in another namespace, add the correct using statement above.
// If it does not exist, define a placeholder below (remove this if you already have the class elsewhere).
public class QuestionUIManager : MonoBehaviour
{
    public void DisplayQuestion(BackendManager.Question question) { }
}

public class GameFlowManager : MonoBehaviour
{
    public BackendManager backend;
    public QuestionUIManager questionUI;

    private List<BackendManager.Question> questions;

    void Start()
    {
        StartCoroutine(LoadQuestions());
    }

    IEnumerator LoadQuestions()
    {
        bool done = false;
        List<BackendManager.Question> qList = null;

        // Use the callback-based GetQuestions
        yield return StartCoroutine(backend.GetQuestions(result => {
            qList = result;
            done = true;
        }));

        yield return new WaitUntil(() => done);

        if (qList != null && qList.Count > 0)
        {
            questions = qList;
            questionUI.DisplayQuestion(questions[0]);
        }
        else
        {
            Debug.LogError("No questions loaded from backend.");
        }
    }
}