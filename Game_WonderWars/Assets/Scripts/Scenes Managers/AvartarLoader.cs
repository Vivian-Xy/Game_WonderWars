using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;

public class AvatarSelector : MonoBehaviour
{
    [Tooltip("Drag your AvatarButton prefab here")]
    public GameObject avatarButtonPrefab;

    [Tooltip("Drag the Content GameObject of your ScrollView here")]
    public Transform contentParent;

    void Start()
    {
        StartCoroutine(LoadAndPopulate());
    }

    IEnumerator LoadAndPopulate()
    {
        // 1. Load all GameObject assets labeled "Avatars"
        var loadAllHandle = Addressables.LoadAssetsAsync<GameObject>(
            /* label = */ "Avatars",
            /* callback = */ null
        );
        yield return loadAllHandle;

        if (loadAllHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[AvatarSelector] Failed to load avatars by label 'Avatars': {loadAllHandle.Status}");
            yield break;
        }

        IList<GameObject> avatarPrefabs = loadAllHandle.Result;
        Debug.Log($"[AvatarSelector] Loaded {avatarPrefabs.Count} avatars with label 'Avatars'");

        // 2. Instantiate one button per avatar prefab
        foreach (var avatarPrefab in avatarPrefabs)
        {
            // 2a. Create the button
            GameObject btnGO = Instantiate(avatarButtonPrefab, contentParent);
            var avatarBtn = btnGO.GetComponent<AvatarButton>();

            // 2b. Grab a thumbnail sprite (e.g. from a SpriteRenderer on the prefab)
            Sprite thumbnail = avatarPrefab.GetComponentInChildren<SpriteRenderer>()?.sprite;

            // 2c. Wire it up
            avatarBtn.Setup(avatarPrefab, thumbnail);
            Debug.Log($"[AvatarSelector] Created button for {avatarPrefab.name}");
        }

        // 3. (Optional) Release the handle if you don't need the list reference afterward
        Addressables.Release(loadAllHandle);
    }
}
