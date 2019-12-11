using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        GameObject player = null;
        var playerHp = 0;

        Entities.WithAll<PlayerData>().ForEach(
            (Entity entity, Transform transform, ref HealthData hp) =>
            {
                player = transform.gameObject;
                playerHp = hp.Value;
            });

        if (player == null)
            return;

        Entities.WithAll<EnemyData>().WithNone<DeadData>().ForEach(
            (Entity entity, NavMeshAgent agent, ref HealthData hp) =>
            {
                if (hp.Value > 0 && playerHp > 0)
                    agent.SetDestination(player.transform.position);
                else
                    agent.enabled = false;
            });
    }
}
