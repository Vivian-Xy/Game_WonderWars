using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Collections;

public class AvatarSelector : MonoBehaviour
{
    public GameObject avatarButtonPrefab;
    public Transform contentParent;

    void Start()
    {
        StartCoroutine(LoadAndPopulate());
    }

    IEnumerator LoadAndPopulate()
    {
        // Only load GameObjects (prefabs), not Sprites or PNGs directly.
        var locationsHandle = Addressables.LoadResourceLocationsAsync("Avatars", typeof(GameObject));
        yield return locationsHandle;

        if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("[AvatarSelector] Failed to load locations for label 'Avatars' and type GameObject");
            yield break;
        }

        var locations = locationsHandle.Result;
        Debug.Log($"[AvatarSelector] Found {locations.Count} avatar prefab locations");

        foreach (IResourceLocation loc in locations)
        {
            var loadHandle = Addressables.LoadAssetAsync<GameObject>(loc);
            yield return loadHandle;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject avatarPrefab = loadHandle.Result;
                Debug.Log($"[AvatarSelector] Loaded avatar prefab: {avatarPrefab.name}");

                var btnGO = Instantiate(avatarButtonPrefab, contentParent);
                var avatarBtn = btnGO.GetComponent<AvatarButton>();

                Sprite thumbnail = avatarPrefab.GetComponentInChildren<SpriteRenderer>()?.sprite;
                avatarBtn.Setup(avatarPrefab, thumbnail);
            }
            else
            {
                Debug.LogError($"[AvatarSelector] Failed to load asset at {loc.PrimaryKey}");
            }
        }

        Addressables.Release(locationsHandle);
    }
}

    
