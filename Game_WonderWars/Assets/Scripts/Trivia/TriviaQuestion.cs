using System.Collections.Generic;

[System.Serializable]
public class TriviaQuestion
{
    public string id;
    public string questionText;
    public List<string> choices;
    public int correctIndex;
    public string rewardPiece;
}
