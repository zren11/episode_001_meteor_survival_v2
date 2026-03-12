using UnityEngine;

// Attach to Food prefab. Requires a Collider with isTrigger = true.
public class FoodItem : MonoBehaviour
{
    private FoodSpawner spawner;

    public void Init(FoodSpawner owner, float lifetime)
    {
        spawner = owner;
        if (lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

    private void OnDestroy()
    {
        spawner?.OnFoodDestroyed(this);
    }
}
