using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the dashboard UI: progress cards, overall chart, history list, and user settings.
/// </summary>
public class DashboardManager : MonoBehaviour
{
    [Header("References")]
    public TriviaManager triviaManager; // drag‑in via Inspector

    [Header("Prefabs & Containers")]
    public GameObject progressCardPrefab;
    public Transform cardsParent;
    public GameObject historyEntryPrefab;
    public Transform historyContent;

    [Header("Overall Progress UI")]
    public Image overallChartFill;
    public TMP_Text overallChartLabel;

    [Header("User Settings")]
    public TMP_InputField usernameInput;
    public TMP_Dropdown avatarDropdown;

    // Internal representations
    private List<MonumentData> monuments;
    private List<HistoryItem> history;

    void Start()
    {
        int savedScore = PlayerPrefs.GetInt("TriviaScore", 0);
        int totalQuestions = 10; // or however many total
        float progress = (float)savedScore / totalQuestions;
        SetChartProgress(progress);

        // Ensure we have a valid TriviaManager
        if (triviaManager == null)
        {
            Debug.LogError("DashboardManager: TriviaManager reference not set.");
            return;
        }

        // Load persistent user settings
        LoadUserSettings();

        // Pull live data from TriviaManager
        LoadMonumentData();
        LoadHistory();

        // Populate UI
        PopulateProgressCards();
        UpdateOverallChart();
        PopulateHistoryList();
    }

    #region User Settings
    private void LoadUserSettings()
    {
        // Username
        usernameInput.text = PlayerPrefs.GetString("Username", "");
        usernameInput.onEndEdit.AddListener(name =>
        {
            PlayerPrefs.SetString("Username", name);
            PlayerPrefs.Save();
        });

        // Avatar selection
        avatarDropdown.value = PlayerPrefs.GetInt("AvatarIndex", 0);
        avatarDropdown.onValueChanged.AddListener(idx =>
        {
            PlayerPrefs.SetInt("AvatarIndex", idx);
            PlayerPrefs.Save();
        });
    }
    #endregion

    #region Data Loading
    private void LoadMonumentData()
    {
        monuments = new List<MonumentData>();

        // Build list based on TriviaManager's progress
        foreach (var mp in triviaManager.monumentProgress)
        {
            monuments.Add(new MonumentData(mp.monumentID, mp.unlockedCount, mp.totalPossible));
        }
    }

    private void LoadHistory()
    {
        history = new List<HistoryItem>();

        // Convert TriviaManager history to DashboardHistoryItems
        foreach (var h in triviaManager.history)
        {
            history.Add(new HistoryItem(h.questionText, h.wasCorrect, h.rewardID));
        }
    }
    #endregion

    #region UI Population
    private void PopulateProgressCards()
    {
        // Clear any existing cards
        foreach (Transform child in cardsParent)
            Destroy(child.gameObject);

        // Instantiate a card for each monument
        foreach (var m in monuments)
        {
            var card = Instantiate(progressCardPrefab, cardsParent);
            card.transform.localScale = Vector3.one;

            // Set icon (expects Resources/Icons/<monumentID>.png)
            var iconImg = card.transform.Find("MonumentIcon").GetComponent<Image>();
            iconImg.sprite = Resources.Load<Sprite>($"Icons/{m.name}");

            // Set name text
            var nameTxt = card.transform.Find("MonumentName").GetComponent<TMP_Text>();
            nameTxt.text = m.name;

            // Set progress bar
            var fillImg = card.transform.Find("BarBackground/BarFill").GetComponent<Image>();
            fillImg.fillAmount = m.total > 0 ? (float)m.unlocked / m.total : 0f;
        }
    }

    private void UpdateOverallChart()
    {
        int totalUnlocked = monuments.Sum(m => m.unlocked);
        int totalPossible = monuments.Sum(m => m.total);
        float pct = totalPossible > 0 ? (float)totalUnlocked / totalPossible : 0f;

        overallChartFill.fillAmount = pct;
        overallChartLabel.text = $"Progress: {(int)(pct * 100)}%";
    }

    public void SetChartProgress(float progress)
    {
        overallChartFill.fillAmount = Mathf.Clamp01(progress); // ensures it's between 0 and 1
    }

    private void PopulateHistoryList()
    {
        // Clear existing entries
        foreach (Transform child in historyContent)
            Destroy(child.gameObject);

        // Instantiate a history entry for each record
        for (int i = 0; i < history.Count; i++)
        {
            var h = history[i];
            var entry = Instantiate(historyEntryPrefab, historyContent);
            entry.transform.localScale = Vector3.one;

            // Set icon
            var icon = entry.transform.Find("ResultIcon").GetComponent<Image>();
            icon.sprite = Resources.Load<Sprite>(h.wasCorrect ? "Icons/check" : "Icons/cross");
            icon.color = h.wasCorrect ? Color.green : Color.red;

            // Set text
            var txt = entry.transform.Find("EntryText").GetComponent<TMP_Text>();
            txt.text = h.questionText;

            // Alternate row tint
            var bg = entry.GetComponent<Image>();
            float alpha = (i % 2 == 0) ? 0.05f : 0.10f;
            bg.color = new Color(1f, 1f, 1f, alpha);
        }
    }
    #endregion

    #region Helper Classes
    private class MonumentData
    {
        public string name;
        public int unlocked;
        public int total;

        public MonumentData(string n, int u, int t)
        {
            name = n; unlocked = u; total = t;
        }
    }

    private class HistoryItem
    {
        public string questionText;
        public bool wasCorrect;
        public string rewardID;

        public HistoryItem(string q, bool c, string r)
        {
            questionText = q;
            wasCorrect = c;
            rewardID = r;
        }
    }
    #endregion
}