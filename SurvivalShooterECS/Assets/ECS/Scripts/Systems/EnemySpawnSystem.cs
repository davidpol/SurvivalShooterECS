using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemySpawnSystem : ComponentSystem
{
    private ComponentGroup spawnerGroup;
    private ComponentGroup playerGroup;

    private readonly List<float> time = new List<float>();

    protected override void OnCreateManager()
    {
        spawnerGroup = GetComponentGroup(ComponentType.Create<EnemySpawner>());
        playerGroup = GetComponentGroup(
            ComponentType.ReadOnly<PlayerData>(),
            ComponentType.ReadOnly<HealthData>());
    }

    protected override void OnUpdate()
    {
        var playerHp = playerGroup.GetComponentDataArray<HealthData>();
        if (playerHp.Length == 0)
            return;

        if (playerHp[0].Value <= 0)
            return;

        var dt = Time.deltaTime;
        var startingHealth = SurvivalShooterBootstrap.Settings.StartingEnemyHealth;

        var spawner = spawnerGroup.GetComponentArray<EnemySpawner>();
        for (var i = 0; i < spawner.Length; i++)
        {
            if (time.Count < i + 1)
                time.Add(0f);

            time[i] += dt;

            if (time[i] >= spawner[i].SpawnTime)
            {
                var enemy = Object.Instantiate(spawner[i].Enemy, spawner[i].transform.position, quaternion.identity);
                var entity = enemy.GetComponent<GameObjectEntity>().Entity;
                EntityManager.AddComponentData(entity, new EnemyData());
                EntityManager.AddComponentData(entity, new HealthData { Value = startingHealth });
                time[i] = 0f;
            }
        }
    }
}
