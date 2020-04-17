using Unity.Entities;
using Unity.Jobs;

public class EnemyHealthSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();
        var dead = GetComponentDataFromEntity<DeadData>();

        Entities
            .WithReadOnly(dead)
            .ForEach((Entity entity, int entityInQueryIndex, ref HealthData healthData, ref DamagedData damagedData) =>
            {
                healthData.Value -= damagedData.Damage;
                ecb.RemoveComponent<DamagedData>(entityInQueryIndex, entity);
                if (healthData.Value <= 0 && !dead.Exists(entity))
                    ecb.AddComponent(entityInQueryIndex, entity, new DeadData());
            }).ScheduleParallel();

        ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
