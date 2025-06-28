using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginSceneUI : MonoBehaviour
{
    public void GoToRegisterScene()
    {
        SceneManager.LoadScene("Register");
    }
}
