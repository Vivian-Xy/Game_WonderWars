using UnityEngine;

/// <summary>
/// Keeps one MusicPlayer alive across scene loads.
/// </summary>
public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;

    void Awake()
    {
        // If another BGMPlayer already exists, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Otherwise, make this the singleton and persist it
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}