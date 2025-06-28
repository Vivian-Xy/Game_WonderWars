using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RegistrationController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button registerButton;
    public Button backButton;
    public TextMeshProUGUI feedbackText;

    void Start()
    {
        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegister);
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners(); // Ensure no duplicate listeners
            backButton.onClick.AddListener(GoToLogin);
        }
    }

    void OnRegister()
    {
        feedbackText.text = "";
        string username = usernameInput != null ? usernameInput.text.Trim() : "";
        string email = emailInput != null ? emailInput.text.Trim() : "";
        string password = passwordInput != null ? passwordInput.text : "";

        // Basic validation
        if (username.Length < 3)
        {
            feedbackText.text = "Username must be at least 3 characters.";
            return;
        }
        if (!email.Contains("@"))
        {
            feedbackText.text = "Please enter a valid email.";
            return;
        }
        if (password.Length < 6)
        {
            feedbackText.text = "Password must be at least 6 characters.";
            return;
        }

        // TODO: call your backend or local data store
        // Replace FakeAuthService with your real backend call if available
        bool success = FakeAuthService.Register(username, email, password);
        if (success)
        {
            // Optionally show feedback before scene change
            feedbackText.text = "Registration successful!";
            // Use the correct scene name for your dashboard/main menu
            SceneManager.LoadScene("Dashboard");
        }
        else
        {
            feedbackText.text = "Registration failed. Try again.";
        }
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("Login");
    }
}

// Example stub service; replace with real API calls
public static class FakeAuthService
{
    public static bool Register(string user, string email, string pass)
    {
        // e.g., PlayerPrefs or API call
        PlayerPrefs.SetString("username", user);
        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", pass);
        return true;
    }
}