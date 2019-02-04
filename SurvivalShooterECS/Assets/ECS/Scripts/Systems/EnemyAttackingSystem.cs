using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class EnemyAttackingSystem : JobComponentSystem
{
    private struct EnemyAttackJob : IJobProcessComponentDataWithEntity<EnemyAttackData>
    {
        public EntityCommandBuffer.Concurrent Ecb;
        
        public float DeltaTime;

        [ReadOnly] public ComponentDataFromEntity<HealthData> Health;
        [ReadOnly] public ComponentDataFromEntity<DamagedData> Damaged;
        
        public void Execute(Entity entity, int index, ref EnemyAttackData attackData)
        {
            attackData.Timer += DeltaTime;

            if (attackData.Timer >= attackData.Frequency &&
                Health.Exists(attackData.Source) &&
                Health[attackData.Source].Value > 0)
            {
                attackData.Timer = 0f;

                var target = attackData.Target;
                if (Health[target].Value > 0 && !Damaged.Exists(target))
                    Ecb.AddComponent(index, target, new DamagedData { Damage = attackData.Damage });
            }
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
        var job = new EnemyAttackJob
        {
            Ecb = barrier.CreateCommandBuffer().ToConcurrent(),
            DeltaTime = Time.deltaTime,
            Health = GetComponentDataFromEntity<HealthData>(),
            Damaged = GetComponentDataFromEntity<DamagedData>()
            
        };
        return job.Schedule(this, inputDeps);
    }
}
