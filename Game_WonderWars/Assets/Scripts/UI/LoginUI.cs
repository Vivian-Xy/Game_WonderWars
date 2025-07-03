using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText;

    // Add these for panel switching
    public GameObject loginPanel;
    public GameObject resetPanel;
    public Button forgotPasswordButton;

    [Header("Backend")]
    public BackendManager backend;

    // These will be set when login succeeds
    private string authToken;
    private string userId;

    // Called by your LoginButton OnClick
    public void OnLoginClick()
    {
        string email = emailInput.text.Trim();
        string pass  = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            feedbackText.text = "Please fill in both fields.";
            return;
        }

        feedbackText.text = "Logging in…";
        StartCoroutine(backend.LoginUser(email, pass, OnLoginResponse));
    }

    // This callback runs when the backend responds
    private void OnLoginResponse(bool success, string json)
    {
        if (!success)
        {
            feedbackText.text = "Login failed: " + json;
            return;
        }

        feedbackText.text = "Login successful!";

        // Parse JSON to extract token and userId
        var data = JsonUtility.FromJson<LoginResponse>(json);
        authToken = data.token;
        userId    = data.user._id; 

        // Persist for other scenes
        PlayerPrefs.SetString("authToken", authToken);
        PlayerPrefs.SetString("userId", userId);

        // Transition to Dashboard
        SceneManager.LoadScene("DashboardScene");
    }

    void Start()
    {
        // show login, hide reset
        if (loginPanel != null) loginPanel.SetActive(true);
        if (resetPanel != null) resetPanel.SetActive(false);

        // wire the forgot‑password link
        if (forgotPasswordButton != null)
        {
            forgotPasswordButton.onClick.AddListener(() => {
                if (loginPanel != null) loginPanel.SetActive(false);
                if (resetPanel != null) resetPanel.SetActive(true);
            });
        }
    }

    // Helper classes to match your backend’s JSON
    [System.Serializable]
    private class LoginResponse
    {
        public string token;
        public UserInfo user;
    }
    [System.Serializable]
    private class UserInfo
    {
        public string _id;
        public string username;
        // add other fields if you need them immediately
    }
}