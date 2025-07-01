// This ScriptableObject is for Unity Editor asset-based questions only.
// For runtime JSON loading, use a plain C# class with matching fields.

using UnityEngine;

[System.Serializable]
public class QuestionData
{
    public string id;
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;
    public string RewardPrefabID;
}

// If you want to use ScriptableObject for editor-based questions, keep this:
[CreateAssetMenu(fileName = "QuestionData", menuName = "Trivia/Question")]
public class QuestionDataSO : ScriptableObject
{
    [TextArea]
    public string questionText;
    public string[] answers = new string[4];
    [Range(0, 3)]
    public int correctAnswerIndex;
    public string RewardPrefabID;
}