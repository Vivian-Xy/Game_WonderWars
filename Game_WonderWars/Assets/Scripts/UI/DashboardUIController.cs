using UnityEngine;
using UnityEngine.SceneManagement;

public class DashboardUIController : MonoBehaviour
{
    /// <summary>
    /// Call this from your “Play Trivia” button.
    /// </summary>
    public void StartTrivia()
    {
        SceneManager.LoadScene("TriviaScene");  // use your exact scene name
    }
}