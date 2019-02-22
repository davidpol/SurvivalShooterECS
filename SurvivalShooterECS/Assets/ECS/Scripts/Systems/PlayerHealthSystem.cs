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
            [ReadOnly] ref PlayerData playerData,
            ref HealthData healthData,
            ref DamagedData damagedData)
        {
            healthData.Value -= damagedData.Damage;
            Ecb.RemoveComponent<DamagedData>(index, entity);
            var e = Ecb.CreateEntity(index, HealthUpdatedArchetype);
            Ecb.SetComponent(index, e, new HealthUpdatedData { Health = healthData.Value });
            if (healthData.Value <= 0 && !Dead.Exists(entity))
                Ecb.AddComponent(index, entity, new DeadData());
        }
    }
    
#pragma warning disable 649
    // ReSharper disable once ClassNeverInstantiated.Local
    private class SystemBarrier : BarrierSystem
    {
    }
    
    [Inject] private SystemBarrier barrier;
#pragma warning restore 649
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new PlayerHealthJob
        {
            Ecb = barrier.CreateCommandBuffer().ToConcurrent(),
            HealthUpdatedArchetype = healthUpdatedArchetype,
            Dead = GetComponentDataFromEntity<DeadData>()
        };
        inputDeps = job.Schedule(this, inputDeps);
        inputDeps.Complete();
        return inputDeps;
    }
}
