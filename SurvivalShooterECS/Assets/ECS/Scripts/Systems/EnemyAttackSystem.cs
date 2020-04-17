using Unity.Entities;
using Unity.Jobs;

public class EnemyAttackSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;
    private EntityArchetype healthUpdatedEventArchetype;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        healthUpdatedEventArchetype = EntityManager.CreateArchetype(typeof(HealthUpdatedEvent));
    }

    protected override void OnUpdate()
    {
        var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();
        var health = GetComponentDataFromEntity<HealthData>();
        var archetypeCopy = healthUpdatedEventArchetype;
        var time = Time.DeltaTime;

        Entities
            .WithReadOnly(health)
            .ForEach((Entity entity, int entityInQueryIndex, ref EnemyAttackData attackData) =>
            {
                attackData.Timer += time;

                var attacker = attackData.Source;
                var target = attackData.Target;
                if (attackData.Timer >= attackData.Frequency &&
                    health[attacker].Value > 0 &&
                    health[target].Value > 0)
                {
                    attackData.Timer = 0f;

                    var newHp = health[target].Value - attackData.Damage;
                    ecb.SetComponent(entityInQueryIndex, target, new HealthData {Value = newHp});

                    var evt = ecb.CreateEntity(entityInQueryIndex, archetypeCopy);
                    ecb.SetComponent(entityInQueryIndex, evt, new HealthUpdatedEvent {Health = newHp});

                    if (newHp <= 0)
                        ecb.AddComponent(entityInQueryIndex, target, new DeadData());
                }
            }).ScheduleParallel();

        ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
