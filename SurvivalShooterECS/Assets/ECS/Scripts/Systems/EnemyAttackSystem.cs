using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class EnemyAttackSystem : JobComponentSystem
{
    private EntityArchetype healthUpdatedEventArchetype;
    private EndSimulationEntityCommandBufferSystem barrier;

    protected override void OnCreate()
    {
        healthUpdatedEventArchetype = EntityManager.CreateArchetype(typeof(HealthUpdatedEvent));
        barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    private struct EnemyAttackJob : IJobForEachWithEntity<EnemyAttackData>
    {
        public EntityCommandBuffer.Concurrent Ecb;

        public float DeltaTime;
        public EntityArchetype HealthUpdatedArchetype;

        [ReadOnly] public ComponentDataFromEntity<HealthData> Health;

        public void Execute(Entity entity, int index, ref EnemyAttackData attackData)
        {
            attackData.Timer += DeltaTime;

            var attacker = attackData.Source;
            var target = attackData.Target;
            if (attackData.Timer >= attackData.Frequency &&
                Health[attacker].Value > 0 &&
                Health[target].Value > 0)
            {
                attackData.Timer = 0f;

                var newHp = Health[target].Value - attackData.Damage;
                Ecb.SetComponent(index, target, new HealthData { Value = newHp });

                var evt = Ecb.CreateEntity(index, HealthUpdatedArchetype);
                Ecb.SetComponent(index, evt, new HealthUpdatedEvent { Health = newHp });

                if (newHp <= 0)
                    Ecb.AddComponent(index, target, new DeadData());
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new EnemyAttackJob
        {
            Ecb = barrier.CreateCommandBuffer().ToConcurrent(),
            DeltaTime = Time.deltaTime,
            HealthUpdatedArchetype = healthUpdatedEventArchetype,
            Health = GetComponentDataFromEntity<HealthData>()
        };
        inputDeps = job.Schedule(this, inputDeps);
        barrier.AddJobHandleForProducer(inputDeps);
        return inputDeps;
    }
}
