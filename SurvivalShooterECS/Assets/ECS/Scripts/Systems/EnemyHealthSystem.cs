using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class EnemyHealthSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem barrier;

    protected override void OnCreateManager()
    {
        barrier = World.GetOrCreateManager<EndSimulationEntityCommandBufferSystem>();
    }

    private struct EnemyHealthJob : IJobProcessComponentDataWithEntity<EnemyData, HealthData, DamagedData>
    {
        public EntityCommandBuffer.Concurrent Ecb;
        
        [ReadOnly] public ComponentDataFromEntity<DeadData> Dead;
        
        public void Execute(
            Entity entity,
            int index,
            [ReadOnly] ref EnemyData enemyData,
            ref HealthData healthData,
            ref DamagedData damagedData)
        {
            healthData.Value -= damagedData.Damage;
            Ecb.RemoveComponent<DamagedData>(index, entity);
            if (healthData.Value <= 0 && !Dead.Exists(entity))
                Ecb.AddComponent(index, entity, new DeadData());
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new EnemyHealthJob
        {
            Ecb = barrier.CreateCommandBuffer().ToConcurrent(),
            Dead = GetComponentDataFromEntity<DeadData>()
        };
        inputDeps = job.Schedule(this, inputDeps);
        barrier.AddJobHandleForProducer(inputDeps);
        return inputDeps;
    }
}
