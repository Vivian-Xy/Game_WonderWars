using UnityEngine;

/// <summary>
/// Singleton to play one-shot sound effects from anywhere.
/// </summary>
public class SFXManager : MonoBehaviour
{
    public static SFXManager I { get; private set; }
    AudioSource src;

    void Awake()
    {
        if (I == null)
        {
            I = this;
            src = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Play a one-shot SFX at given volume (0–1).
    /// </summary>
    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        src.PlayOneShot(clip, volume);
    }
}