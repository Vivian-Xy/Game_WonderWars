using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthUIManager : MonoBehaviour
{
    public TMP_InputField usernameInput, emailInput, passwordInput;
    public TMP_Dropdown avatarDropdown;
    public TMP_Text feedbackText;
    public BackendManager backend;

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

    public void OnLoginClick()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

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

    public void GoToRegister()
    {
        SceneManager.LoadScene("RegisterScene");
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }
}