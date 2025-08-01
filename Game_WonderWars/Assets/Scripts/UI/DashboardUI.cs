using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class DashboardUI : MonoBehaviour
{
    [Header("UI Refs")]
    public TMP_Text welcomeText;
    public Image avatarImage;
    public TMP_Text scoreText;
    public TMP_Text progressList;
    public Button logoutButton;
    public Button playTriviaButton; // Assign in Inspector

    [Header("Backend")]
    public BackendManager backend;

    private string userId;
    private string authToken;

    void Start()
    {
        // 1) Get saved credentials
        authToken = PlayerPrefs.GetString("authToken", "");
        userId    = PlayerPrefs.GetString("userId", "");

        // Defensive null checks
        if (backend == null)
        {
            Debug.LogError("DashboardUI: BackendManager reference not set.");
            enabled = false;
            return;
        }
        if (logoutButton != null)
            logoutButton.onClick.AddListener(OnLogout);
        if (playTriviaButton != null)
            playTriviaButton.onClick.AddListener(OnPlayTrivia);

        // 3) Fetch data only if credentials are present
        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(authToken))
        {
            StartCoroutine(backend.GetUserData(userId, authToken, OnUserDataResponse));
        }
        else
        {
            welcomeText.text = "Not logged in.";
        }
    }

    void OnLogout()
    {
        PlayerPrefs.DeleteKey("authToken");
        PlayerPrefs.DeleteKey("userId");
        SceneManager.LoadScene("LoginScene");
    }

    void OnPlayTrivia()
    {
        // Defensive: ensure the scene exists and is spelled correctly
        SceneManager.LoadScene("TriviaScene");
    }

    // Callback from BackendManager
    void OnUserDataResponse(bool success, string json)
    {
        if (!success)
        {
            welcomeText.text = "Error loading data";
            return;
        }

        // Parse JSON
        var resp = JsonUtility.FromJson<UserDataResponse>(json);

        welcomeText.text = $"Welcome, {resp.user.username}!";
        scoreText.text   = resp.user.score.ToString();

        // Load avatar sprite by name if you have a map, e.g.:
        // avatarImage.sprite = AvatarLibrary.GetSprite(resp.user.avatar);

        // List unlocked monuments
        progressList.text = string.Join(", ", resp.user.unlockedMonuments);
    }

    // Data models matching your API
    [System.Serializable]
    private class UserDataResponse
    {
        public User user;
    }
    [System.Serializable]
    private class User
    {
        public string _id;
        public string username;
        public int score;
        public string avatar;
        public string[] unlockedMonuments;
    }
}

/* Removed duplicate DashboardUI class definition. 
   If you want StartTrivia() functionality, move it into the main DashboardUI class above. */