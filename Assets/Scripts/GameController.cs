using UnityEngine;

/// <summary>
/// Game controller — spawns flowers, hazards, and wind zones at intervals.
/// Matches the original game.gd logic from the Godot version.
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject flowerPrefab;
    public GameObject hazardPrefab;
    public GameObject windZonePrefab;
    public Transform flowerSpawner;
    public Transform hazardSpawner;
    public Transform windSpawner;

    [Header("Timing")]
    public float spawnInterval = 1.2f;
    public float hazardInterval = 3.0f;
    public float windInterval = 4.0f;

    private Camera cam;
    private float screenHalfWidth;
    private float screenTop;
    private float gameTime;
    private float difficulty = 1f;
    private float spawnTimer;
    private float hazardTimer;
    private float windTimer;

    private void Start()
    {
        cam = Camera.main;
        screenHalfWidth = cam.orthographicSize * cam.aspect;
        screenTop = cam.orthographicSize;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;

        gameTime += Time.deltaTime;
        difficulty = 1.0f + gameTime * 0.02f;

        // Spawn flowers
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval / difficulty && flowerPrefab != null)
        {
            spawnTimer = 0f;
            SpawnObject(flowerPrefab, flowerSpawner);
        }

        // Spawn hazards
        hazardTimer += Time.deltaTime;
        if (hazardTimer >= hazardInterval / (difficulty * 0.5f) && hazardPrefab != null)
        {
            hazardTimer = 0f;
            SpawnObject(hazardPrefab, hazardSpawner);
        }

        // Spawn wind zones
        windTimer += Time.deltaTime;
        if (windTimer >= windInterval && windZonePrefab != null)
        {
            windTimer = 0f;
            SpawnObject(windZonePrefab, windSpawner);
        }
    }

    private void SpawnObject(GameObject prefab, Transform parent)
    {
        float x = Random.Range(-screenHalfWidth + 1f, screenHalfWidth - 1f);
        float y = screenTop + 2f;

        Vector3 pos = new Vector3(x, y, 0f);
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity, parent);
    }
}
