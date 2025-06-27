using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RegisterUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Dropdown avatarDropdown;
    public TMP_Text feedbackText;

    [Header("Backend")]
    public BackendManager backendManager; // <-- Make sure this is public and not [SerializeField] private

    // Called by your RegisterButton OnClick
    public void OnRegisterClick()
    {
        string user  = usernameInput.text.Trim();
        string mail  = emailInput.text.Trim();
        string pass  = passwordInput.text;
        string avatar = avatarDropdown.options[avatarDropdown.value].text;

        if (string.IsNullOrEmpty(user) ||
            string.IsNullOrEmpty(mail) ||
            string.IsNullOrEmpty(pass))
        {
            feedbackText.text = "All fields are required.";
            return;
        }

        feedbackText.text = "Registering…";
        StartCoroutine(backendManager.RegisterUser(
            user, mail, pass, avatar,
            OnRegisterResponse
        ));
    }

    // Callback from BackendManager
    private void OnRegisterResponse(bool success, string json)
    {
        if (success)
        {
            feedbackText.text = "Registration successful!";
            // Optionally auto-login or redirect to LoginScene
            SceneManager.LoadScene("LoginScene");
        }
        else
        {
            feedbackText.text = "Error: " + json;
        }
    }
}