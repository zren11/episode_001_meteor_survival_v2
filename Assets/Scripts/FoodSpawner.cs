using System.Collections.Generic;
using UnityEngine;

// Attach to SurvivalEnv root (or any stable parent).
// Manages food spawn lifecycle for one episode.
public class FoodSpawner : MonoBehaviour
{
    [Header("References")]
    public TrainingConfig config;
    public GameObject foodPrefab;
    // Center of the spawn area. Assign the Arena transform or leave null to use this transform.
    public Transform spawnAreaCenter;

    private readonly List<FoodItem> activeFoods = new();

    // Called by SurvivalAgent.OnEpisodeBegin
    public void ResetForEpisode()
    {
        StopSpawning();
        ClearAllFoods();
        StartSpawning();
    }

    private void StartSpawning()
    {
        InvokeRepeating(nameof(TrySpawn), 0f, config.spawnInterval);
    }

    private void StopSpawning()
    {
        CancelInvoke(nameof(TrySpawn));
    }

    private void ClearAllFoods()
    {
        foreach (var food in activeFoods)
        {
            if (food != null)
                Destroy(food.gameObject);
        }
        activeFoods.Clear();
    }

    private void TrySpawn()
    {
        // Clean up any nulls from lifetime-expired foods
        activeFoods.RemoveAll(f => f == null);

        if (activeFoods.Count >= config.maxConcurrentFoods)
            return;

        float half = config.arenaHalfSize - config.foodSpawnEdgePadding;
        Vector3 center = spawnAreaCenter != null ? spawnAreaCenter.position : transform.position;
        Vector3 spawnPos = new Vector3(
            center.x + Random.Range(-half, half),
            config.foodSpawnY,
            center.z + Random.Range(-half, half)
        );

        GameObject go = Instantiate(foodPrefab, spawnPos, Quaternion.identity, transform);
        FoodItem item = go.GetComponent<FoodItem>();
        item.Init(this, config.foodLifetimeSeconds);
        activeFoods.Add(item);
    }

    // Called by SurvivalAgent when food is eaten
    public void EatFood(FoodItem item)
    {
        if (!activeFoods.Contains(item))
            return;

        activeFoods.Remove(item);
        Destroy(item.gameObject);
    }

    // Called by FoodItem.OnDestroy (lifetime expiry path)
    public void OnFoodDestroyed(FoodItem item)
    {
        activeFoods.Remove(item);
    }
}
