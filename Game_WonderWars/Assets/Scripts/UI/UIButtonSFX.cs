using UnityEngine;

/// <summary>
/// Attach to a UI Button to play an AudioClip when clicked.
/// </summary>
public class UIButtonSFX : MonoBehaviour
{
    [Tooltip("The sound to play when this button is clicked")]
    public AudioClip clip;

    [Range(0f, 1f), Tooltip("Volume of the click sound")]
    public float volume = 1f;

    /// <summary>
    /// Call this method via the Button's OnClick() event.
    /// </summary>
    public void Play()
    {
        if (clip != null && SFXManager.I != null)
            SFXManager.I.Play(clip, volume);
    }
}