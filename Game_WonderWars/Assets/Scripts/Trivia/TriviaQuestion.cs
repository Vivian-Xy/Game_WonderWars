using UnityEngine;

[System.Serializable]
public class TriviaQuestion
{
    // The text of the question itself
    public string questionText;

    // An array of possible answers (e.g., {"A", "B", "C", "D"})
    public string[] answers;

    // The index into that array which is “correct” (0‐based)
    public int correctAnswerIndex;
}