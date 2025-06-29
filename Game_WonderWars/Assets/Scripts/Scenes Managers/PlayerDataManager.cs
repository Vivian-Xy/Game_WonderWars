using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    private const string PREFS_KEY = "EarnedPieces";
    private List<string> earnedPieces = new List<string>();

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData();
    }

    /// <summary>
    /// Adds a piece if not already earned, then saves.
    /// </summary>
    public void AddPiece(string pieceName)
    {
        if (earnedPieces.Contains(pieceName)) return;

        earnedPieces.Add(pieceName);
        SaveData();
        Debug.Log($"[PlayerData] Added piece: {pieceName}");
    }

    /// <summary>
    /// Returns a copy of the earned pieces list.
    /// </summary>
    public List<string> GetAllPieces()
    {
        return new List<string>(earnedPieces);
    }

    /// <summary>
    /// Saves the earnedPieces list to PlayerPrefs as a JSON string.
    /// </summary>
    private void SaveData()
    {
        string json = JsonUtility.ToJson(new Wrapper { pieces = earnedPieces });
        PlayerPrefs.SetString(PREFS_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the earnedPieces list from PlayerPrefs.
    /// </summary>
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(PREFS_KEY))
        {
            string json = PlayerPrefs.GetString(PREFS_KEY);
            var wrapper = JsonUtility.FromJson<Wrapper>(json);
            earnedPieces = wrapper.pieces ?? new List<string>();
        }
        else
        {
            earnedPieces = new List<string>();
        }
        Debug.Log($"[PlayerData] Loaded {earnedPieces.Count} pieces.");
    }

    // Helper class for JSON serialization
    [System.Serializable]
    private class Wrapper
    {
        public List<string> pieces;
    }
}