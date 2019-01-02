using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class PlayerHealthSystem : JobComponentSystem
{
    private EntityArchetype healthUpdatedArchetype;
    
    protected override void OnCreateManager()
    {
        healthUpdatedArchetype = EntityManager.CreateArchetype(typeof(HealthUpdatedData));
    }

    private struct PlayerHealthJob : IJobProcessComponentDataWithEntity<PlayerData, HealthData, DamagedData>
    {
        public EntityCommandBuffer.Concurrent Ecb;

        public EntityArchetype HealthUpdatedArchetype;
        [ReadOnly] public ComponentDataFromEntity<DeadData> Dead;
        
        public void Execute(
            Entity entity,
            int index,
            ref PlayerData playerData,
            ref HealthData healthData,
            ref DamagedData damagedData)
        {
            var newHealth = healthData.Value -= damagedData.Damage;
            Ecb.SetComponent(index, entity, new HealthData { Value = newHealth });
            Ecb.RemoveComponent(index, entity, typeof(DamagedData));
            Ecb.CreateEntity(index, HealthUpdatedArchetype);
            Ecb.SetComponent(index, new HealthUpdatedData { Health = newHealth });
            if (newHealth <= 0 && !Dead.Exists(entity))
                Ecb.AddComponent(index, entity, new DeadData());
        }
    }
    
#pragma warning disable 649
    [Inject] private EndFrameBarrier endFrameBarrier;
#pragma warning restore 649
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new PlayerHealthJob
        {
            Ecb = endFrameBarrier.CreateCommandBuffer().ToConcurrent(),
            HealthUpdatedArchetype = healthUpdatedArchetype,
            Dead = GetComponentDataFromEntity<DeadData>()
        };
        return job.Schedule(this, inputDeps);
    }
}
