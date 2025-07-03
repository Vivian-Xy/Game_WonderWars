using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneUI : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject resetPanel;
    public Button forgotPasswordButton;

    void Start()
    {
        // Show login, hide reset at start
        if (loginPanel != null) loginPanel.SetActive(true);
        if (resetPanel != null) resetPanel.SetActive(false);

        // Wire the forgot-password link
        if (forgotPasswordButton != null)
        {
            forgotPasswordButton.onClick.AddListener(() => {
                if (loginPanel != null) loginPanel.SetActive(false);
                if (resetPanel != null) resetPanel.SetActive(true);
            });
        }
    }

    public void GoToRegisterScene()
    {
        SceneManager.LoadScene("Register");
    }
}
