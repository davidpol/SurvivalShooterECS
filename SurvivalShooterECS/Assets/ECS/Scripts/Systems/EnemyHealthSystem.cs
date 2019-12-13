using Unity.Entities;
using Unity.Jobs;

public class EnemyHealthSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();
        var dead = GetComponentDataFromEntity<DeadData>();

        var jobHandle = Entities
            .WithReadOnly(dead)
            .ForEach((Entity entity, int entityInQueryIndex, ref HealthData healthData, ref DamagedData damagedData) =>
            {
                healthData.Value -= damagedData.Damage;
                ecb.RemoveComponent<DamagedData>(entityInQueryIndex, entity);
                if (healthData.Value <= 0 && !dead.Exists(entity))
                    ecb.AddComponent(entityInQueryIndex, entity, new DeadData());
            }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}
