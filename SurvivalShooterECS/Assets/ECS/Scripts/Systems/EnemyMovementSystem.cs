using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementSystem : ComponentSystem
{ 
    private ComponentGroup enemyGroup;
    private ComponentGroup playerGroup;

    protected override void OnCreateManager()
    {
        enemyGroup = GetComponentGroup(
            ComponentType.ReadOnly<EnemyData>(),
            ComponentType.ReadOnly<HealthData>(),
            ComponentType.ReadOnly<NavMeshAgent>(),
            ComponentType.Exclude<DeadData>());
        playerGroup = GetComponentGroup(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<PlayerData>(),
            ComponentType.ReadOnly<HealthData>());
    }

    protected override void OnUpdate()
    {
        GameObject player = null;
        var playerHp = 0;

        Entities.With(playerGroup).ForEach(
            (Entity entity, Transform transform, ref HealthData hp) =>
            {
                player = transform.gameObject;
                playerHp = hp.Value;
            });

        if (player == null)
            return;

        Entities.With(enemyGroup).ForEach(
            (Entity entity, NavMeshAgent agent, ref HealthData hp) =>
            {
                if (hp.Value > 0 && playerHp > 0)
                    agent.SetDestination(player.transform.position);
                else
                    agent.enabled = false;
            });
    }
}
