using UnityEngine;

[CreateAssetMenu(fileName = "TrainingConfig", menuName = "MeteorSurvival/TrainingConfig")]
public class TrainingConfig : ScriptableObject
{
    [Header("Agent")]
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;
    public float maxHp = 100f;
    public float episodeTimeCap = 60f;
    public float arenaHalfSize = 10f;

    [Header("Food")]
    public float spawnInterval = 3f;
    public int maxConcurrentFoods = 5;
    // -1 = permanent (no auto-destroy)
    public float foodLifetimeSeconds = -1f;
    public float foodEatReward = 1f;
    public float foodSpawnEdgePadding = 1f;
    public float foodSpawnY = 0.55f;

    [Header("Reward")]
    public float survivalRewardPerSecond = 0.01f;
    public float idlePenaltyPerSecond = 0f;
    public float outOfBoundsPenalty = -1f;

    [Header("Stage")]
    public int currentStage = 1;
}
