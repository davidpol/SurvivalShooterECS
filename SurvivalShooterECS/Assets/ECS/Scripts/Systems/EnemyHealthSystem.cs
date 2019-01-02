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
            ref EnemyData enemyData,
            ref HealthData healthData,
            ref DamagedData damagedData)
        {
            var newHealth = healthData.Value -= damagedData.Damage;
            Ecb.SetComponent(index, entity, new HealthData { Value = newHealth });
            Ecb.RemoveComponent(index, entity, typeof(DamagedData));
            if (newHealth <= 0 && !Dead.Exists(entity))
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
