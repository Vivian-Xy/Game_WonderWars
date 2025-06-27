using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFlowManager : MonoBehaviour
{
    public BackendManager backend;
    public QuestionUIManager questionUI;

    void Start()
    {
        StartCoroutine(LoadQuestions());
    }

    IEnumerator LoadQuestions()
    {
        yield return StartCoroutine(backend.GetQuestions());

        List<BackendManager.Question> qList = backend.GetComponent<QuestionCache>().questions;
        questionUI.DisplayQuestion(qList[0]); // Start with first question
    }
}