using UnityEngine;

[CreateAssetMenu(fileName = "QuestionData", menuName = "Trivia/Question")]
public class QuestionData : ScriptableObject
{
    [TextArea]
    public string questionText;
    public string[] answers = new string[4];
    [Range(0, 3)]
    public int correctAnswerIndex;
    public string rewardPrefabID;
}