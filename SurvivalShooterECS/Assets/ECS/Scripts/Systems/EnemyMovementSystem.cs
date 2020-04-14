using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        GameObject player = null;
        var playerHp = 0;

        Entities.WithoutBurst().WithAll<PlayerData>().ForEach(
            (Entity entity, Transform transform, ref HealthData hp) =>
            {
                player = transform.gameObject;
                playerHp = hp.Value;
            }).Run();

        if (player == null)
            return;

        Entities.WithoutBurst().WithAll<EnemyData>().WithNone<DeadData>().ForEach(
            (Entity entity, NavMeshAgent agent, ref HealthData hp) =>
            {
                if (hp.Value > 0 && playerHp > 0)
                    agent.SetDestination(player.transform.position);
                else
                    agent.enabled = false;
            }).Run();
    }
}
