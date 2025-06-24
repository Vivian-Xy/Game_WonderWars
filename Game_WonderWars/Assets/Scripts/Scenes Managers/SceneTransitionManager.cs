using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton manager for smooth scene transitions with fade in/out.
/// Attach this script to the root of TransitionCanvas prefab.
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Fade Settings")]
    [Tooltip("UI Image used for full-screen fade")]    
    public Image fadeImage;
    [Tooltip("Duration of fade in seconds")]    
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Ensure starting fully opaque
            if (fadeImage != null)
                fadeImage.canvasRenderer.SetAlpha(1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Automatically fade in at scene start
        if (fadeImage != null)
            StartCoroutine(Fade(1f, 0f));
    }

    /// <summary>
    /// Public method to load a new scene with fade
    /// </summary>
    /// <param name="sceneName">Name of the scene to load (without .unity)</param>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("SceneTransitionManager: LoadScene called with empty sceneName");
            return;
        }
        // If for some reason this GameObject was disabled, re-enable it:
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        StartCoroutine(PerformTransition(sceneName));
    }

    // Internal coroutine to handle fade out, load, and fade in
    private IEnumerator PerformTransition(string sceneName)
    {
        // Fade to black
        if (fadeImage != null)
            yield return Fade(1f, 0f); // fade from opaque (1) to transparent (0) before loading

        // Load the scene asynchronously
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
            yield return null;

        // Fade back in
        if (fadeImage != null)
            yield return Fade(0f, 1f); // fade from transparent (0) to opaque (1) after loading
    }

    // Coroutine to interpolate alpha of fadeImage
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadeImage.canvasRenderer.SetAlpha(a);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeImage.canvasRenderer.SetAlpha(endAlpha);
    }
}