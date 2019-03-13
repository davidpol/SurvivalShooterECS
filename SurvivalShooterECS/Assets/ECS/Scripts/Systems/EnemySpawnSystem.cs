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
        spawnerGroup = GetComponentGroup(
            ComponentType.ReadOnly<EnemySpawner>());
        playerGroup = GetComponentGroup(
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

        if (player == null || playerHp <= 0)
            return;

        var dt = Time.deltaTime;
        var startingHealth = SurvivalShooterBootstrap.Settings.StartingEnemyHealth;

        var spawner = spawnerGroup.ToComponentArray<EnemySpawner>();
        for (var i = 0; i < spawner.Length; i++)
        {
            if (time.Count < i + 1)
                time.Add(0f);

            time[i] += dt;

            if (time[i] >= spawner[i].SpawnTime)
            {
                var enemy = Object.Instantiate(spawner[i].Enemy, spawner[i].transform.position, quaternion.identity);
                var entity = enemy.GetComponent<GameObjectEntity>().Entity;
                PostUpdateCommands.AddComponent(entity, new EnemyData());
                PostUpdateCommands.AddComponent(entity, new HealthData { Value = startingHealth });
                time[i] = 0f;
            }
        }
    }
}
