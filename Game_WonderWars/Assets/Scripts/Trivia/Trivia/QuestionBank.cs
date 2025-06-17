using UnityEngine;

[CreateAssetMenu(fileName = "QuestionBank", menuName = "Trivia/Bank")]
public class QuestionBank : ScriptableObject {
    public QuestionData[] questions;
}