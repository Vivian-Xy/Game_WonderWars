using UnityEngine;
using UnityEngine.UI;

public class AuthUIManager : MonoBehaviour
{
    public InputField usernameInput, emailInput, passwordInput;
    public Dropdown avatarDropdown;
    public BackendManager backend;

    public void OnRegisterClick()
    {
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;
        string avatar = avatarDropdown.options[avatarDropdown.value].text;

        StartCoroutine(backend.RegisterUser(username, email, password, avatar));
    }

    public void OnLoginClick()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        StartCoroutine(backend.LoginUser(email, password));
    }
}