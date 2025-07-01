using UnityEngine;

/// <summary>
/// Singleton manager for handling player avatar selection and profile data.
/// </summary>
public class PlayerProfileManager : MonoBehaviour
{
    public static PlayerProfileManager Instance { get; private set; }

    public GameObject currentAvatarPrefab { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Call this to set the player's avatar prefab.
    /// </summary>
    public void ChooseAvatar(GameObject avatarPrefab)
    {
        currentAvatarPrefab = avatarPrefab;
        // Optionally save avatar selection to PlayerPrefs or update UI here
        Debug.Log($"PlayerProfileManager: Avatar selected: {avatarPrefab?.name}");
    }
}
