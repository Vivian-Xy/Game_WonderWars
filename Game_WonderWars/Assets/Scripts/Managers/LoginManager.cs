using UnityEngine;
using UnityEngine.UI;               // for Button
using TMPro;                        // for TMP_InputField & TextMeshProUGUI
using UnityEngine.SceneManagement;  // for SceneManager.LoadScene

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput; // drag UsernameInput here
    public TMP_InputField passwordInput; // drag PasswordInput here
    public Button loginButton;            // drag LoginButton here
    public TextMeshProUGUI errormessageText;  // drag errorMessageText here

    [Header("Demo Credentials")]
    public string validUsername = "player";
    public string validPassword = "pass123";

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        errormessageText.text = ""; // Clear feedback text at start
    }

    void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errormessageText.text = "Please enter both username and password.";
            return;
        }

        // Simulate a successful login (replace with actual authentication logic)
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.SetString("Password", password);

        errormessageText.text = "Login successful!";

        // Load the main game scene (replace with your actual scene name)
        SceneManager.LoadScene("TriviaScene");
    }
    /// <summary>
    /// Called when the user clicks “Login”. Reads the input fields, validates them,
    /// and either loads the TriviaScene or displays an error message.
    /// </summary>
    private void OnLoginButtonPressed()
    {
        // 1) Read the user’s input
        string enteredUser = "";
        string enteredPass = "";

        if (usernameInput != null)
            enteredUser = usernameInput.text.Trim();  // Trim whitespace

        if (passwordInput != null)
            enteredPass = passwordInput.text;

        // 2) Check credentials
        if (enteredUser == validUsername && enteredPass == validPassword)
        {
            // Credentials are correct → load the Trivia scene
            SceneManager.LoadScene("TriviaScene");
        }
        else
        {
            // Invalid credentials → show an error
            if (errormessageText != null)
                errormessageText.text = "Invalid username or password";
        }
    }
}