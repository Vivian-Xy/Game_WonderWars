using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DashboardManager : MonoBehaviour
{
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

    // Internal data
    private List<MonumentData> monuments;
    private List<HistoryItem> history;

    void Start()
    {
        LoadUserSettings();
        LoadMonumentData();
        LoadHistory();

        PopulateProgressCards();
        UpdateOverallChart();
        PopulateHistoryList();
    }

    void LoadUserSettings()
    {
        usernameInput.text = PlayerPrefs.GetString("Username", "");
        avatarDropdown.value = PlayerPrefs.GetInt("AvatarIndex", 0);

        usernameInput.onEndEdit.AddListener(name =>
            PlayerPrefs.SetString("Username", name));
        avatarDropdown.onValueChanged.AddListener(idx =>
            PlayerPrefs.SetInt("AvatarIndex", idx));
    }

    void LoadMonumentData()
    {
        monuments = new List<MonumentData>();
        // TODO: Replace these mock values with real data
        monuments.Add(new MonumentData("Pyramid", 3, 5));
        monuments.Add(new MonumentData("Colosseum", 2, 5));
        // …add one entry per monument
    }

    void LoadHistory()
    {
        history = new List<HistoryItem>();
        // TODO: Replace with actual question/answer records
        // history.Add(new HistoryItem("What country are the Pyramids in?", true));
    }

    void PopulateProgressCards()
    {
        // Clear existing cards (optional)
        foreach (Transform child in cardsParent) Destroy(child.gameObject);

        foreach (var m in monuments)
        {
            var card = Instantiate(progressCardPrefab, cardsParent);
            card.transform.localScale = Vector3.one;

            // Set icon
            var icon = card.transform.Find("MonumentIcon").GetComponent<Image>();
            icon.sprite = Resources.Load<Sprite>($"Icons/{m.name}");

            // Set name
            var nameText = card.transform.Find("MonumentName").GetComponent<TMP_Text>();
            nameText.text = m.name;

            // Set fill amount
            float pct = (float)m.unlocked / m.total;
            var fill = card.transform.Find("BarBackground/BarFill").GetComponent<Image>();
            fill.fillAmount = pct;
        }
    }

    void UpdateOverallChart()
    {
        int totalUnlocked = 0, grandTotal = 0;
        foreach (var m in monuments)
        {
            totalUnlocked += m.unlocked;
            grandTotal += m.total;
        }
        float pct = grandTotal > 0 ? (float)totalUnlocked / grandTotal : 0f;
        overallChartFill.fillAmount = pct;
        overallChartLabel.text = $"Progress: {(int)(pct * 100)}%";
    }

    void PopulateHistoryList()
    {
        // Clear existing entries (optional)
        foreach (Transform child in historyContent) Destroy(child.gameObject);

        foreach (var h in history)
        {
            var entry = Instantiate(historyEntryPrefab, historyContent);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<TMP_Text>().text =
                $"{h.questionText} – {(h.wasCorrect ? "✔" : "❌")}";
        }
    }

    // Data classes
    public class MonumentData
    {
        public string name;
        public int unlocked, total;
        public MonumentData(string n, int u, int t)
        {
            name = n; unlocked = u; total = t;
        }
    }

    public class HistoryItem
    {
        public string questionText;
        public bool wasCorrect;
        public HistoryItem(string q, bool c)
        {
            questionText = q; wasCorrect = c;
        }
    }
}