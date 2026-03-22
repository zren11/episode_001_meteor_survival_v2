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

    [Header("HP Drain")]
    // HP lost per second passively; Agent restores full HP on eating food
    public float hpDrainPerSecond = 5f;
    // Reward penalty applied at the same rate as HP drain
    public float hpDrainPenaltyPerSecond = 0.05f;

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
    public float cliffPenalty = -1f;
    public float cliffFallThresholdY = -1f;

    [Header("Stage")]
    public int currentStage = 1;
}
