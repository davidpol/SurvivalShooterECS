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
            ComponentType.Create<EnemyData>(),
            ComponentType.Create<HealthData>(),
            ComponentType.Create<NavMeshAgent>(),
            ComponentType.Subtractive<DeadData>());
        playerGroup = GetComponentGroup(
            ComponentType.Create<Transform>(),
            ComponentType.Create<PlayerData>(),
            ComponentType.Create<HealthData>());
    }

    protected override void OnUpdate()
    {
        var player = playerGroup.GetGameObjectArray();
        var playerHp = playerGroup.GetComponentDataArray<HealthData>();
        var agent = enemyGroup.GetComponentArray<NavMeshAgent>();
        var enemyHp = enemyGroup.GetComponentDataArray<HealthData>();
        for (var i = 0; i < agent.Length; i++)
        {
            if (enemyHp[i].Value > 0 && playerHp[0].Value > 0)
                agent[i].SetDestination(player[0].transform.position);
            else
                agent[i].enabled = false;
        }
    }
}
