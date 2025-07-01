using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

    [Header("Inventory UI")]
    public GameObject rewardIconPrefab;  // assign RewardIcon.prefab
    public Transform inventoryPanel;      // assign InventoryPanel.transform

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
            enabled = false;
            return;
        }

        // Check all required references before proceeding
        if (
            progressCardPrefab == null || cardsParent == null ||
            historyEntryPrefab == null || historyContent == null ||
            rewardIconPrefab == null || inventoryPanel == null ||
            overallChartFill == null || overallChartLabel == null ||
            usernameInput == null || avatarDropdown == null
        )
        {
            Debug.LogError("DashboardManager: One or more UI references are not set in the Inspector.");
            foreach (var go in new Object[] { progressCardPrefab, cardsParent, historyEntryPrefab, historyContent, rewardIconPrefab, inventoryPanel, overallChartFill, overallChartLabel, usernameInput, avatarDropdown })
            {
                if (go == null)
                    Debug.LogError($"DashboardManager: Missing reference: {go}");
            }
            enabled = false;
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
        PopulateInventory();
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
        if (monuments == null)
        {
            Debug.LogError("DashboardManager: monuments list is null.");
            return;
        }
        if (progressCardPrefab == null || cardsParent == null)
        {
            Debug.LogError("DashboardManager: progressCardPrefab or cardsParent is null.");
            return;
        }

        Debug.Log($"PopulateProgressCards: {monuments.Count} monuments, prefab={(progressCardPrefab==null?"NULL":progressCardPrefab.name)}, parent={(cardsParent==null?"NULL":cardsParent.name)}");

        // Clear old cards
        foreach (Transform child in cardsParent)
            Destroy(child.gameObject);

        // Instantiate each card with a staggered pop-in
        for (int i = 0; i < monuments.Count; i++)
        {
            var m = monuments[i];
            var card = Instantiate(progressCardPrefab, cardsParent);
            card.transform.localScale = Vector3.one;

            // Defensive null checks for child objects
            var iconImg = card.transform.Find("MonumentIcon")?.GetComponent<Image>();
            if (iconImg != null)
                iconImg.sprite = Resources.Load<Sprite>($"Icons/{m.name}");

            var nameTxt = card.transform.Find("MonumentName")?.GetComponent<TMP_Text>();
            if (nameTxt != null)
                nameTxt.text = m.name;

            var fillImg = card.transform.Find("BarBackground/BarFill")?.GetComponent<Image>();
            if (fillImg != null)
                fillImg.fillAmount = m.total > 0 ? (float)m.unlocked / m.total : 0f;

            var cg = card.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(i * 0.1f);
                seq.Append(cg.DOFade(1f, 0.2f).SetEase(Ease.Linear));
            }
        }
    }

    private void UpdateOverallChart()
    {
        if (monuments == null || overallChartFill == null || overallChartLabel == null)
            return;

        int totalUnlocked = monuments.Sum(m => m.unlocked);
        int totalPossible = monuments.Sum(m => m.total);
        float pct = totalPossible > 0 ? (float)totalUnlocked / totalPossible : 0f;

        overallChartFill.fillAmount = pct;
        overallChartLabel.text = $"Progress: {(int)(pct * 100)}%";
    }

    public void SetChartProgress(float progress)
    {
        if (overallChartFill != null)
            overallChartFill.fillAmount = Mathf.Clamp01(progress);
    }

    private void PopulateHistoryList()
    {
        if (history == null || historyEntryPrefab == null || historyContent == null)
            return;

        foreach (Transform child in historyContent)
            Destroy(child.gameObject);

        for (int i = 0; i < history.Count; i++)
        {
            var h = history[i];
            var entry = Instantiate(historyEntryPrefab, historyContent);
            entry.transform.localScale = Vector3.one;

            var icon = entry.transform.Find("ResultIcon")?.GetComponent<Image>();
            if (icon != null)
            {
                icon.sprite = Resources.Load<Sprite>(h.wasCorrect ? "Icons/check" : "Icons/cross");
                icon.color = h.wasCorrect ? Color.green : Color.red;
            }

            var txt = entry.transform.Find("EntryText")?.GetComponent<TMP_Text>();
            if (txt != null)
                txt.text = h.questionText;

            var bg = entry.GetComponent<Image>();
            if (bg != null)
            {
                float alpha = (i % 2 == 0) ? 0.05f : 0.10f;
                bg.color = new Color(1f, 1f, 1f, alpha);
            }
        }
    }

    public void PopulateInventory()
    {
        if (inventoryPanel == null || rewardIconPrefab == null || monuments == null)
            return;

        foreach (Transform child in inventoryPanel)
            Destroy(child.gameObject);

        foreach (var m in monuments)
        {
            for (int i = 0; i < m.unlocked; i++)
            {
                var icon = Instantiate(rewardIconPrefab, inventoryPanel);
                icon.transform.localScale = Vector3.one;
                var img = icon.GetComponent<Image>();
                if (img != null)
                    img.sprite = Resources.Load<Sprite>($"Icons/{m.name}");
            }
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

    /// <summary>
    /// Clears saved score and reward progress, then refreshes the dashboard UI.
    /// </summary>
    public void ResetProgress()
    {
        // Clear saved score
        PlayerPrefs.DeleteKey("TriviaScore");

        // Clear each monument’s saved unlock count
        foreach (var m in triviaManager.monumentProgress)
        {
            PlayerPrefs.DeleteKey($"Unlocked_{m.monumentID}");
            m.unlockedCount = 0;  // reset in-memory too
        }

        PlayerPrefs.Save();

        // Reload live data
        LoadMonumentData();
        LoadHistory();

        // Refresh UI
        PopulateProgressCards();
        UpdateOverallChart();
        PopulateHistoryList();
        PopulateInventory();
    }

    public void LoadScene(string sceneName)
    {
        // If for some reason this GameObject was disabled, re-enable it:
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        StartCoroutine(PerformTransition(sceneName));
    }

    private IEnumerator PerformTransition(string sceneName)
    {
        // Optional: Add transition animation or fade-out here
        yield return null; // Placeholder for animation duration

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}