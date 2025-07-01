using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Call Setup(prefab, thumbnail) right after instantiating this button.
/// </summary>
[RequireComponent(typeof(Button))]
public class AvatarButton : MonoBehaviour
{
    public Image avatarImage;       // Assign the ThumbnailImage here
    private GameObject avatarPrefab; // The actual avatar to spawn

    /// <summary>
    /// Configure this button to represent a specific avatar.
    /// </summary>
    public void Setup(GameObject prefab, Sprite thumbnail)
    {
        avatarPrefab = prefab;
        avatarImage.sprite = thumbnail;

        // Clear old click listeners
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        // Add new one
        btn.onClick.AddListener(OnAvatarButtonClicked);
    }

    private void OnAvatarButtonClicked()
    {
        // Notify your profile manager
        PlayerProfileManager.Instance.ChooseAvatar(avatarPrefab);
        // Optionally: update a preview UI, close the picker, etc.
    }
}
