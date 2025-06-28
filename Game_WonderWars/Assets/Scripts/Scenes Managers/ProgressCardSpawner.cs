using UnityEngine;

public class ProgressCardSpawner : MonoBehaviour
{
    public static ProgressCardSpawner Instance { get; private set; }
    public GameObject cardPrefab;  // Assign your ProgressCard prefab here
    private Transform canvasParent;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        // Find existing Canvas in scene
        Canvas c = FindFirstObjectByType<Canvas>();
        canvasParent = c.transform;
    }

    public void SpawnCard(string title, Sprite thumbnail)
    {
        GameObject cardGO = Instantiate(cardPrefab, canvasParent);
        ProgressCardController controller = cardGO.GetComponent<ProgressCardController>();
        controller.Show(title, thumbnail);
    }
}