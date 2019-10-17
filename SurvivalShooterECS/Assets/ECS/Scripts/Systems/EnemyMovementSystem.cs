using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementSystem : ComponentSystem
{
    private EntityQuery enemyQuery;
    private EntityQuery playerQuery;

    protected override void OnCreate()
    {
        enemyQuery = GetEntityQuery(
            ComponentType.ReadOnly<EnemyData>(),
            ComponentType.ReadOnly<HealthData>(),
            ComponentType.ReadOnly<NavMeshAgent>(),
            ComponentType.Exclude<DeadData>());
        playerQuery = GetEntityQuery(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<PlayerData>(),
            ComponentType.ReadOnly<HealthData>());
    }

    protected override void OnUpdate()
    {
        GameObject player = null;
        var playerHp = 0;

        Entities.With(playerQuery).ForEach(
            (Entity entity, Transform transform, ref HealthData hp) =>
            {
                player = transform.gameObject;
                playerHp = hp.Value;
            });

        if (player == null)
            return;

        Entities.With(enemyQuery).ForEach(
            (Entity entity, NavMeshAgent agent, ref HealthData hp) =>
            {
                if (hp.Value > 0 && playerHp > 0)
                    agent.SetDestination(player.transform.position);
                else
                    agent.enabled = false;
            });
    }
}
