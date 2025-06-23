using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Centralized UI color definitions for consistent theming.
/// </summary>
public static class UIColors
{
    public static Color Primary = new Color32(0, 150, 255, 255); // Bright Blue
    public static Color Success = new Color32(0, 200, 0, 255); // Green
    public static Color Danger = new Color32(200, 0, 0, 255); // Red
    public static Color PanelBg = new Color32(240, 240, 240, 255); // Light Gray
    public static Color PanelAlt = new Color32(255, 255, 255, 20); // Very Subtle White
    public static Color TextDark = new Color32(20, 20, 20, 255); // Almost Black
    public static Color TextLight = new Color32(255, 255, 255, 255); // White
}

// Example usage of UIColors in a MonoBehaviour
public class SomeUISetup : MonoBehaviour
{
    public Image panelImage;
    public TMP_Text headerText;

    void Awake()
    {
        panelImage.color = UIColors.PanelBg;
        headerText.color = UIColors.Primary;
    }
}
