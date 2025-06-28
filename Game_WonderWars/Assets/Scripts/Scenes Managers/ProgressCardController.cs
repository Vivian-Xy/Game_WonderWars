using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ProgressCardController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public Image thumbnailImage;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        HideInstant();
    }

    // Show with fade-in
    public void Show(string title, Sprite thumbnail, float duration = 2f)
    {
        titleText.text = title;
        thumbnailImage.sprite = thumbnail;
        StopAllCoroutines();
        StartCoroutine(FadeInThenOut(duration));
    }

    private IEnumerator FadeInThenOut(float displayTime)
    {
        // Fade IN
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            canvasGroup.alpha = t / 0.3f;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Wait
        yield return new WaitForSeconds(displayTime);

        // Fade OUT
        for (float t = 1f; t > 0; t -= Time.deltaTime / 0.3f)
        {
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private void HideInstant()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    public void Close()
    {
        // Option A: simply hide it for reuse
        gameObject.SetActive(false);

        // Option B: destroy it completely
        // Destroy(gameObject);
    }
}