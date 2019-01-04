using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class EnemyHealthSystem : JobComponentSystem
{
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
    
#pragma warning disable 649
    [Inject] private EndFrameBarrier endFrameBarrier;
#pragma warning restore 649
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new EnemyHealthJob
        {
            Ecb = endFrameBarrier.CreateCommandBuffer().ToConcurrent(),
            Dead = GetComponentDataFromEntity<DeadData>()
        };
        return job.Schedule(this, inputDeps);
    }
}
