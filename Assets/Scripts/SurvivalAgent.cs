using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;

// Attach to Agent GameObject.
// Requires: Rigidbody, Collider (non-trigger), BehaviorParameters, DecisionRequester
//
// Observation space: 7 floats
//   [0-1] velocity (x, z) normalized by moveSpeed
//   [2-3] forward direction (x, z)
//   [4-5] relative position to arena center, normalized by arenaHalfSize
//   [6]   normalized episode time
//
// Action space: 2 continuous
//   [0] forward/backward (-1 to 1)
//   [1] turn left/right (-1 to 1)
public class SurvivalAgent : Agent
{
    [Header("Config")]
    public TrainingConfig config;

    [Header("References")]
    public FoodSpawner foodSpawner;
    // Assign SurvivalEnv root transform (the arena center reference)
    public Transform arenaCenter;

    private Rigidbody rb;
    private float currentHp;
    private float episodeTimer;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset position inside arena
        float half = config.arenaHalfSize - config.foodSpawnEdgePadding;
        Vector3 center = arenaCenter != null ? arenaCenter.position : Vector3.zero;
        transform.position = center + new Vector3(
            Random.Range(-half, half),
            0f,
            Random.Range(-half, half)
        ) + Vector3.up * 0.55f;

        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        currentHp = config.maxHp;
        episodeTimer = 0f;

        foodSpawner.ResetForEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 vel = rb.linearVelocity;
        sensor.AddObservation(vel.x / config.moveSpeed);       // [0]
        sensor.AddObservation(vel.z / config.moveSpeed);       // [1]

        sensor.AddObservation(transform.forward.x);            // [2]
        sensor.AddObservation(transform.forward.z);            // [3]

        Vector3 center = arenaCenter != null ? arenaCenter.position : Vector3.zero;
        Vector3 rel = transform.position - center;
        sensor.AddObservation(rel.x / config.arenaHalfSize);   // [4]
        sensor.AddObservation(rel.z / config.arenaHalfSize);   // [5]

        sensor.AddObservation(episodeTimer / config.episodeTimeCap); // [6]
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float dt = Time.fixedDeltaTime;
        episodeTimer += dt;

        float moveForward = actions.ContinuousActions[0];
        float turn = actions.ContinuousActions[1];

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn * config.turnSpeed * dt, 0f));
        rb.MovePosition(rb.position + transform.forward * moveForward * config.moveSpeed * dt);

        // Survival reward
        AddReward(config.survivalRewardPerSecond * dt);

        // Idle penalty
        if (config.idlePenaltyPerSecond > 0f && Mathf.Abs(moveForward) < 0.1f)
            AddReward(-config.idlePenaltyPerSecond * dt);

        // Out of bounds → punish and end
        if (IsOutOfBounds())
        {
            AddReward(config.outOfBoundsPenalty);
            EndEpisode();
            return;
        }

        // HP check (structure ready for Stage 2 damage)
        if (currentHp <= 0f)
        {
            EndEpisode();
            return;
        }

        // Time cap
        if (episodeTimer >= config.episodeTimeCap)
            EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ca = actionsOut.ContinuousActions;
        var kb = Keyboard.current;
        if (kb == null) return;

        ca[0] = (kb.wKey.isPressed ? 1f : 0f) + (kb.sKey.isPressed ? -1f : 0f);
        ca[1] = (kb.dKey.isPressed ? 1f : 0f) + (kb.aKey.isPressed ? -1f : 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        FoodItem food = other.GetComponent<FoodItem>();
        if (food != null)
        {
            AddReward(config.foodEatReward);
            foodSpawner.EatFood(food);
        }
    }

    private bool IsOutOfBounds()
    {
        Vector3 center = arenaCenter != null ? arenaCenter.position : Vector3.zero;
        Vector3 rel = transform.position - center;
        float half = config.arenaHalfSize;
        return Mathf.Abs(rel.x) > half || Mathf.Abs(rel.z) > half;
    }
}
