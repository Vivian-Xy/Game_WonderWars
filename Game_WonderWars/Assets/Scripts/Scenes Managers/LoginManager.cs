using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Manages the login UI: captures username (and optional avatar choice), persists user settings, and transitions to the main trivia scene.
/// </summary>
public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_Dropdown avatarDropdown;   // optional avatar selection
    public Button loginButton;

    [Header("Scene Management")]
    [Tooltip("Name of the scene to load after successful login")]
    public string nextSceneName = "DashboardScene";

    void Start()
    {
        // Preload previously saved username and avatar index
        usernameInput.text = PlayerPrefs.GetString("Username", "");
        if (avatarDropdown != null)
            avatarDropdown.value = PlayerPrefs.GetInt("AvatarIndex", 0);

        // Hook up button listener
        loginButton.onClick.RemoveAllListeners();
        loginButton.onClick.AddListener(OnLoginClicked);
    }

    /// <summary>
    /// Called when the user clicks the Login button.
    /// Validates inputs, saves to PlayerPrefs, and loads the next scene.
    /// </summary>
    public void OnLoginClicked()
    {
        string username = usernameInput.text.Trim();
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("LoginManager: Username is empty.");
            // Optionally show UI feedback here
            return;
        }

        // Save username
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.SetString("LoggedInUser", username); // Also save as "LoggedInUser"

        // Save avatar choice if available
        if (avatarDropdown != null)
        {
            PlayerPrefs.SetInt("AvatarIndex", avatarDropdown.value);
        }

        PlayerPrefs.Save();

        // Only load the dashboard scene once, using SceneTransitionManager if available
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (SceneTransitionManager.Instance != null)
                SceneTransitionManager.Instance.LoadScene(nextSceneName);
            else
                SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("LoginManager: nextSceneName not set.");
        }
    }
}