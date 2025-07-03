using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class AuthUIManager : MonoBehaviour
{
    public TMP_InputField usernameInput, emailInput, passwordInput;
    public TMP_Dropdown avatarDropdown;
    public TMP_Text feedbackText;
    public BackendManager backend;
    public GameObject forgotPasswordPopup; // Assign a UI panel in inspector

    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public Button loginButton; // Assign in Inspector

    void Start()
    {
        // Ensure login button is enabled and wired up
        if (loginButton != null)
        {
            loginButton.onClick.RemoveAllListeners();
            loginButton.onClick.AddListener(OnLoginClick);
            loginButton.interactable = true;
        }
    }

    public void OnRegisterClick()
    {
        string username = usernameInput.text.Trim();
        string email = emailInput.text.Trim();
        string password = passwordInput.text;
        string avatar = avatarDropdown.options[avatarDropdown.value].text;

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

        feedbackText.text = "Registering…";
        StartCoroutine(backend.RegisterUser(
            username, email, password, avatar,
            (success, msg) =>
            {
                if (success)
                {
                    feedbackText.text = "Registration successful!";
                    SceneManager.LoadScene("Login");
                }
                else
                {
                    feedbackText.text = "Error: " + msg;
                }
            }
        ));
    }

    // Only use one login method for clarity.
    // If you want to use backend authentication, use OnLoginClick().
    // If you want to use hardcoded credentials for testing, use OnLoginClicked().
    // Remove or comment out one to avoid confusion.

    // Example: Use backend login as the main method.
    public void OnLoginClick()
    {
        string email = emailInput != null ? emailInput.text.Trim() : emailField.text.Trim();
        string password = passwordInput != null ? passwordInput.text : passwordField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please fill in both fields.";
            return;
        }

        feedbackText.text = "Logging in…";
        StartCoroutine(backend.LoginUser(email, password, (success, msg) =>
        {
            if (success)
            {
                feedbackText.text = "Login successful!";
                SceneManager.LoadScene("DashboardScene");
            }
            else
            {
                feedbackText.text = "Login failed: " + msg;
            }
        }));
    }

    // If you want to keep the hardcoded login for testing, you can use this method separately.
    // public void OnLoginClicked()
    // {
    //     string email = emailField.text.Trim();
    //     string password = passwordField.text;
    //     if (email == "user@example.com" && password == "password123")
    //     {
    //         SceneManager.LoadScene("DashboardScene");
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Invalid login credentials");
    //     }
    // }

    public void GoToRegister()
    {
        SceneManager.LoadScene("RegisterScene");
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void OnForgotPasswordClicked()
    {
        if (forgotPasswordPopup != null)
            forgotPasswordPopup.SetActive(true);
    }

    // Example usage to disable post-processing on a camera:
    public void DisableCameraPostProcessing(Camera camera)
    {
        var camData = camera.GetComponent<UniversalAdditionalCameraData>();
        if (camData != null)
            camData.renderPostProcessing = false;
    }
}